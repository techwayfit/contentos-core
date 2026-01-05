# Background Jobs & Scheduled Tasks Design

**Status:** Design Proposal  
**Date:** 2026-01-05  
**Related:** ADR-006 (Event-Driven), ADR-010 (Polyglot Persistence)

---

## Overview

ContentOS requires background job processing for:
- **Scheduled tasks** (cleanup, archival, report generation)
- **Long-running operations** (bulk imports, exports, processing)
- **Recurring jobs** (daily reports, weekly backups, hourly indexing)
- **Delayed execution** (send email in 10 minutes, retry after 5 min)
- **Event-driven async work** (search indexing after publish, embedding generation)

---

## Architecture Principles

### 1. **Multi-Tenant Safe**
- Every job is scoped by `tenantId`
- Jobs cannot access other tenant data
- Queue isolation per tenant (optional)

### 2. **Provider-Agnostic**
- Abstract interface in `TechWayFit.ContentOS.Abstractions`
- Multiple implementations:
  - `InMemory` - Development/testing
  - `PostgreSQL` - Single-server production
  - `Hangfire` - Advanced scheduling (optional)
  - `Azure Service Bus / RabbitMQ` - Distributed systems

### 3. **Event-Driven Integration**
- Event handlers can enqueue background jobs
- Jobs can publish domain events when complete

### 4. **Observable & Debuggable**
- Job execution history/audit trail
- Failed job retry with exponential backoff
- Dead letter queue for permanently failed jobs

---

## Database Schema

### Separation of Concerns

The schema is split into **three tables**:

1. **JOB_DEFINITION** - Job metadata (what to run, when, how often)
2. **JOB_EXECUTION** - Individual execution instances (runtime state)
3. **JOB_EXECUTION_HISTORY** - Long-term audit trail (completed executions)

This separation enables:
- ✅ One job definition, many executions
- ✅ Clear distinction between "job exists" vs "job is running"
- ✅ Recurring jobs create new executions without modifying definition
- ✅ Web farm safety with distributed locks

---

### JOB_DEFINITION

**Purpose:** Job metadata and scheduling configuration.
- Defines WHAT to run and WHEN
- Immutable once created (except for enable/disable)
- One row per unique job (even recurring jobs)

```sql
CREATE TABLE JOB_DEFINITION (
  -- Identity
  id UUID PRIMARY KEY,
  tenant_id UUID NOT NULL REFERENCES TENANT(id),
  
  -- Job identification
  job_name VARCHAR(200) NOT NULL,           -- 'CleanupExpiredTokens', 'GenerateMonthlyReport'
  job_key VARCHAR(200) NOT NULL,            -- Unique key for idempotency: 'cleanup-tokens'
  job_type VARCHAR(500) NOT NULL,           -- Fully qualified type name
  job_parameters JSONB,                     -- Default/template parameters
  
  -- Scheduling configuration
  schedule_type VARCHAR(50) NOT NULL,       -- 'Once', 'Recurring', 'Cron', 'Delayed', 'Manual'
  cron_expression VARCHAR(100),             -- For recurring: '0 0 * * *', '*/5 * * * *'
  interval_seconds INT,                     -- Alternative to cron: run every N seconds
                                            -- For 'Manual': triggered via API or by other jobs
  
  -- Execution settings
  priority INT DEFAULT 0,                   -- Higher = runs first (0-100)
  execution_scope VARCHAR(50) DEFAULT 'Cluster', -- 'Cluster' (run once) or 'Instance' (run on every server)
  max_retries INT DEFAULT 3,
  timeout_seconds INT DEFAULT 3600,         -- Kill job if exceeds this
  max_concurrent_executions INT DEFAULT 1,  -- 0 = unlimited, 1 = sequential only
  
  -- State
  is_enabled BOOLEAN DEFAULT true,          -- Can be disabled without deleting
  last_execution_id UUID,                   -- FK to most recent JOB_EXECUTION
  next_run_at TIMESTAMP,                    -- When next execution should occur
  
  -- Metadata
  created_on TIMESTAMP NOT NULL,
  created_by UUID,
  updated_on TIMESTAMP,
  is_deleted BOOLEAN DEFAULT false,
  deleted_on TIMESTAMP,
  
  -- Constraints
  UNIQUE (tenant_id, job_key) WHERE deleted_on IS NULL,
  
  -- Indexes
  INDEX idx_job_def_next_run (tenant_id, is_enabled, next_run_at) 
    WHERE is_enabled = true AND is_deleted = false,
  INDEX idx_job_def_type (tenant_id, job_type),
  INDEX idx_job_def_key (tenant_id, job_key)
);
```

---

### JOB_EXECUTION

**Purpose:** Individual job execution instances with distributed lock support.
- One row per execution attempt
- Supports web farm scenarios with row-level locking
- Worker claims job by acquiring lock

```sql
CREATE TABLE JOB_EXECUTION (
  -- Identity
  id UUID PRIMARY KEY,
  tenant_id UUID NOT NULL REFERENCES TENANT(id),
  job_definition_id UUID NOT NULL REFERENCES JOB_DEFINITION(id),
  
  -- Execution context
  execution_number INT NOT NULL,            -- 1st, 2nd, 3rd attempt for this scheduled time
  scheduled_at TIMESTAMP NOT NULL,          -- When this execution was supposed to run
  enqueued_at TIMESTAMP NOT NULL,           -- When it was added to queue
  
  -- Distributed lock (WEB FARM SUPPORT)
  status VARCHAR(50) NOT NULL,              -- 'Pending', 'Claimed', 'Running', 'Completed', 'Failed', 'Cancelled', 'TimedOut'
  claimed_by VARCHAR(200),                  -- Worker instance ID (hostname, pod name, etc.)
  claimed_at TIMESTAMP,                     -- When worker claimed this job
  lock_expires_at TIMESTAMP,                -- Heartbeat timeout (if NOW() > this, job is orphaned)
  heartbeat_at TIMESTAMP,                   -- Last heartbeat from worker
  
  -- Execution tracking
  started_at TIMESTAMP,
  completed_at TIMESTAMP,
  duration_ms BIGINT,
  
  -- Results
  result_data JSONB,                        -- Success data
  error_message TEXT,
  error_type VARCHAR(200),                  -- Exception type for retry logic
  stack_trace TEXT,
  
  -- Retry tracking
  is_retry BOOLEAN DEFAULT false,
  retry_of_execution_id UUID,               -- If retry, links to original execution
  retry_count INT DEFAULT 0,
  can_retry BOOLEAN DEFAULT true,
  
  -- Metadata
  created_on TIMESTAMP NOT NULL,
  
  -- Constraints
  FOREIGN KEY (retry_of_execution_id) REFERENCES JOB_EXECUTION(id),
  
  -- Indexes
  INDEX idx_job_exec_pending (tenant_id, status, scheduled_at)
    WHERE status = 'Pending',
  
  INDEX idx_job_exec_claimed (claimed_by, status, lock_expires_at)
    WHERE status IN ('Claimed', 'Running'),
  
  INDEX idx_job_exec_orphaned (status, lock_expires_at)
    WHERE status IN ('Claimed', 'Running'),
    
  INDEX idx_job_exec_definition (job_definition_id, created_on DESC),
  
  INDEX idx_job_exec_retry (retry_of_execution_id)
);
```

---

### JOB_EXECUTION_HISTORY

**Purpose:** Long-term audit trail (optional, for compliance/analytics).
- Completed/failed executions moved here for archival
- Keeps JOB_EXECUTION table lean
- Can be partitioned by date for performance

```sql
CREATE TABLE JOB_EXECUTION_HISTORY (
  -- Same schema as JOB_EXECUTION
  id UUID PRIMARY KEY,
  tenant_id UUID NOT NULL,
  job_definition_id UUID NOT NULL,
  job_name VARCHAR(200),                    -- Denormalized for queries
  job_type VARCHAR(500),                    -- Denormalized
  
  execution_number INT,
  scheduled_at TIMESTAMP,
  started_at TIMESTAMP,
  completed_at TIMESTAMP,
  duration_ms BIGINT,
  
  status VARCHAR(50),
  claimed_by VARCHAR(200),
  
  result_data JSONB,
  error_message TEXT,
  error_type VARCHAR(200),
  
  retry_count INT,
  
  archived_at TIMESTAMP NOT NULL,           -- When moved to history
  
  -- Indexes
  INDEX idx_job_history_tenant_date (tenant_id, archived_at DESC),
  INDEX idx_job_history_definition (job_definition_id, archived_at DESC),
  INDEX idx_job_history_status (tenant_id, status, archived_at DESC)
)
PARTITION BY RANGE (archived_at);            -- Monthly partitions for scalability

-- Create partitions
CREATE TABLE JOB_EXECUTION_HISTORY_2026_01 PARTITION OF JOB_EXECUTION_HISTORY
  FOR VALUES FROM ('2026-01-01') TO ('2026-02-01');
```

---

## Web Farm Support (Distributed Execution)

### Problem: Multiple Servers, Same Job

In a web farm with 3 servers, all polling for jobs:

```
Server A ──┐
Server B ──┼──→ Database ─→ Same job "Pending"
Server C ──┘
           
❌ Problem: All 3 servers try to execute the same job!
```

---

### Execution Scopes

Background jobs can have two execution scopes:

| Scope | Behavior | Use Cases | Implementation |
|-------|----------|-----------|----------------|
| **Cluster** (default) | Run **once** across entire farm | • Send emails<br>• Generate reports<br>• Process payments<br>• Index content<br>• Generate embeddings | Distributed lock (one server claims) |
| **Instance** | Run on **every server** | • Clear local cache<br>• Health checks<br>• Collect local metrics<br>• Cleanup temp files<br>• Refresh in-memory config | No lock (all servers execute) |

**Configuration in JOB_DEFINITION:**
```sql
-- Cluster-scoped (default): Run once across farm
INSERT INTO JOB_DEFINITION (..., execution_scope) 
VALUES (..., 'Cluster');

-- Instance-scoped: Run on every server
INSERT INTO JOB_DEFINITION (..., execution_scope) 
VALUES (..., 'Instance');
```

---

### Solution for Cluster-Scoped Jobs: Optimistic Locking with Row-Level Locks

Use PostgreSQL's `SELECT ... FOR UPDATE SKIP LOCKED` to ensure only ONE server claims a job:

```sql
-- Worker claims CLUSTER-SCOPED job atomically
BEGIN;

-- Claim job (only one server succeeds due to row lock)
UPDATE JOB_EXECUTION
SET 
  status = 'Claimed',
  claimed_by = :worker_instance_id,        -- 'server-a-pod-123'
  claimed_at = NOW(),
  lock_expires_at = NOW() + INTERVAL '5 minutes',  -- Heartbeat timeout
  heartbeat_at = NOW()
WHERE id = (
  SELECT je.id 
  FROM JOB_EXECUTION je
  JOIN JOB_DEFINITION jd ON je.job_definition_id = jd.id
  WHERE je.tenant_id = :tenant_id
    AND je.status = 'Pending'
    AND je.scheduled_at <= NOW()
    AND jd.execution_scope = 'Cluster'     -- ← Only cluster-scoped jobs
  ORDER BY 
    jd.priority DESC,
    je.scheduled_at ASC
  LIMIT 1
  FOR UPDATE SKIP LOCKED                   -- ← KEY: Skip locked rows
)
RETURNING *;

COMMIT;
```

---

### Solution for Instance-Scoped Jobs: No Lock Required

For jobs that must run on every server, each worker claims its own execution:

```sql
-- Each worker creates its own execution for instance-scoped jobs
BEGIN;

-- Check if this worker already has an execution for this scheduled time
INSERT INTO JOB_EXECUTION (
  id, tenant_id, job_definition_id, execution_number,
  scheduled_at, enqueued_at, status, claimed_by, claimed_at
)
SELECT 
  gen_random_uuid(),
  :tenant_id,
  jd.id,
  1,
  jd.next_run_at,
  NOW(),
  'Claimed',
  :worker_instance_id,
  NOW()
FROM JOB_DEFINITION jd
WHERE jd.is_enabled = true
  AND jd.execution_scope = 'Instance'      -- ← Instance-scoped only
  AND jd.next_run_at <= NOW()
  AND NOT EXISTS (
    -- Don't create duplicate execution for this worker + scheduled time
    SELECT 1 FROM JOB_EXECUTION je
    WHERE je.job_definition_id = jd.id
      AND je.claimed_by = :worker_instance_id
      AND je.scheduled_at = jd.next_run_at
  )
RETURNING *;

COMMIT;
```

**Result:**
```
Worker A: Creates execution for Job X (scheduled_at: 12:00)
Worker B: Creates execution for Job X (scheduled_at: 12:00)
Worker C: Creates execution for Job X (scheduled_at: 12:00)

All 3 executions run independently ✅
```

**How `FOR UPDATE SKIP LOCKED` works:**

```
Time →
─────────────────────────────────────────────────────────────
Server A: SELECT ... FOR UPDATE SKIP LOCKED
          ↓ Acquires lock on row 1
          ✅ Claims Job ID 1

Server B: SELECT ... FOR UPDATE SKIP LOCKED (1ms later)
          ↓ Row 1 locked by A, SKIPS IT
          ✅ Claims Job ID 2 (different job)

Server C: SELECT ... FOR UPDATE SKIP LOCKED (2ms later)
          ↓ Rows 1, 2 locked, SKIPS THEM
          ✅ Claims Job ID 3

Result: No duplicate execution! Each server gets different job.
```

---

### Heartbeat Mechanism

**Purpose:** Detect crashed workers and reclaim orphaned jobs.

**Worker Heartbeat Loop:**
```csharp
public class JobWorker
{
    private async Task ExecuteJobWithHeartbeatAsync(JobExecution job)
    {
        using var cts = new CancellationTokenSource();
        
        // Start heartbeat background task
        var heartbeatTask = Task.Run(async () =>
        {
            while (!cts.Token.IsCancellationRequested)
            {
                await UpdateHeartbeatAsync(job.Id);
                await Task.Delay(TimeSpan.FromSeconds(30), cts.Token);
            }
        }, cts.Token);
        
        try
        {
            // Update status to Running
            await UpdateStatusAsync(job.Id, "Running");
            
            // Execute actual job
            await ExecuteAsync(job);
            
            // Mark as completed
            await UpdateStatusAsync(job.Id, "Completed");
        }
        finally
        {
            cts.Cancel(); // Stop heartbeat
        }
    }
    
    private async Task UpdateHeartbeatAsync(Guid executionId)
    {
        await _db.ExecuteAsync(@"
            UPDATE JOB_EXECUTION
            SET heartbeat_at = NOW(),
                lock_expires_at = NOW() + INTERVAL '5 minutes'
            WHERE id = @id AND status IN ('Claimed', 'Running')",
            new { id = executionId }
        );
    }
}
```

**Orphaned Job Reclaimer (separate background service):**
```csharp
public class OrphanedJobReclaimer : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // Find CLUSTER-SCOPED jobs with expired locks (worker crashed/killed)
            var orphanedJobs = await _db.QueryAsync<JobExecution>(@"
                SELECT je.* FROM JOB_EXECUTION je
                JOIN JOB_DEFINITION jd ON je.job_definition_id = jd.id
                WHERE je.status IN ('Claimed', 'Running')
                  AND je.lock_expires_at < NOW()
                  AND je.can_retry = true
                  AND jd.execution_scope = 'Cluster'    -- Only reclaim cluster-scoped
            ");
            
            foreach (var job in orphanedJobs)
            {
                // Reset to Pending for retry
                await _db.ExecuteAsync(@"
                    UPDATE JOB_EXECUTION
                    SET status = 'Pending',
                        claimed_by = NULL,
                        claimed_at = NULL,
                        lock_expires_at = NULL,
                        heartbeat_at = NULL,
                        retry_count = retry_count + 1
                    WHERE id = @id",
                    new { job.Id }
                );
                
                _logger.LogWarning(
                    "Reclaimed orphaned job {JobId} from {Worker}", 
                    job.Id, 
                    job.ClaimedBy
                );
            }
            
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}
```

---

### Job Scheduler Logic

The scheduler creates new JOB_EXECUTION records when it's time to run a job. Logic differs by execution scope:

#### Cluster-Scoped Jobs (Run Once)

```csharp
public class JobScheduler : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // Create executions for cluster-scoped jobs
            await _db.ExecuteAsync(@"
                INSERT INTO JOB_EXECUTION (
                  id, tenant_id, job_definition_id, execution_number,
                  scheduled_at, enqueued_at, status
                )
                SELECT 
                  gen_random_uuid(),
                  tenant_id,
                  id,
                  1,
                  next_run_at,
                  NOW(),
                  'Pending'
                FROM JOB_DEFINITION
                WHERE is_enabled = true
                  AND execution_scope = 'Cluster'
                  AND next_run_at <= NOW()
                  AND NOT EXISTS (
                    -- Avoid duplicate executions for same scheduled time
                    SELECT 1 FROM JOB_EXECUTION
                    WHERE job_definition_id = JOB_DEFINITION.id
                      AND scheduled_at = JOB_DEFINITION.next_run_at
                  )
            ");
            
            // Update next_run_at for recurring jobs
            await UpdateNextRunTimeAsync();
            
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }
}
```

**Result:** ONE execution created, multiple workers compete to claim it.

#### Instance-Scoped Jobs (Run on Every Server)

```csharp
public class JobWorker : BackgroundService
{
    private readonly string _workerInstanceId = WorkerInstanceId.Generate();
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // Create executions for instance-scoped jobs (per worker)
            await _db.ExecuteAsync(@"
                INSERT INTO JOB_EXECUTION (
                  id, tenant_id, job_definition_id, execution_number,
                  scheduled_at, enqueued_at, status, claimed_by, claimed_at
                )
                SELECT 
                  gen_random_uuid(),
                  jd.tenant_id,
                  jd.id,
                  1,
                  jd.next_run_at,
                  NOW(),
                  'Claimed',
                  @worker_instance_id,
                  NOW()
                FROM JOB_DEFINITION jd
                WHERE jd.is_enabled = true
                  AND jd.execution_scope = 'Instance'
                  AND jd.next_run_at <= NOW()
                  AND NOT EXISTS (
                    -- Each worker creates its own execution (once per scheduled time)
                    SELECT 1 FROM JOB_EXECUTION je
                    WHERE je.job_definition_id = jd.id
                      AND je.claimed_by = @worker_instance_id
                      AND je.scheduled_at = jd.next_run_at
                  )
                RETURNING *
            ", new { worker_instance_id = _workerInstanceId });
            
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }
}
```

**Result:** MULTIPLE executions created (one per worker), each worker executes its own.

---

### Concurrency Control Patterns

#### Pattern 1: Sequential Only (max_concurrent_executions = 1)

**Use case:** Jobs that must NOT run in parallel (e.g., database migrations)

```sql
-- Before creating new execution, check if job is already running
INSERT INTO JOB_EXECUTION (...)
SELECT ...
WHERE NOT EXISTS (
  SELECT 1 FROM JOB_EXECUTION e
  WHERE e.job_definition_id = :job_def_id
    AND e.status IN ('Pending', 'Claimed', 'Running')
)
AND (
  SELECT max_concurrent_executions FROM JOB_DEFINITION WHERE id = :job_def_id
) = 1;
```

#### Pattern 2: Limited Concurrency (max_concurrent_executions = 5)

**Use case:** Resource-intensive jobs (e.g., max 5 exports at once)

```sql
-- Count running executions before allowing new one
INSERT INTO JOB_EXECUTION (...)
SELECT ...
WHERE (
  SELECT COUNT(*) FROM JOB_EXECUTION e
  WHERE e.job_definition_id = :job_def_id
    AND e.status IN ('Claimed', 'Running')
) < (
  SELECT max_concurrent_executions FROM JOB_DEFINITION WHERE id = :job_def_id
);
```

#### Pattern 3: Tenant-Level Isolation

**Ensure tenant A's jobs don't block tenant B's jobs:**

```sql
-- Partition executions by tenant
SELECT * FROM JOB_EXECUTION
WHERE tenant_id = :tenant_id
  AND status = 'Pending'
ORDER BY 
  (SELECT priority FROM JOB_DEFINITION WHERE id = job_definition_id) DESC
LIMIT 10
FOR UPDATE SKIP LOCKED;
```

---

### Scaling Strategies

#### Horizontal Scaling (Multiple Workers)

```
┌─────────────┐   ┌─────────────┐   ┌─────────────┐
│  Worker 1   │   │  Worker 2   │   │  Worker 3   │
│  (Server A) │   │  (Server B) │   │  (Server C) │
└──────┬──────┘   └──────┬──────┘   └──────┬──────┘
       │                 │                 │
       └─────────────────┼─────────────────┘
                         │
                    ┌────▼────┐
                    │PostgreSQL│
                    │ (shared) │
                    └─────────┘

✅ Each worker claims different jobs via SKIP LOCKED
✅ No coordination needed between workers
✅ Add/remove workers dynamically
```

**Worker Instance ID:**
```csharp
public class WorkerInstanceId
{
    public static string Generate()
    {
        // Unique ID per worker instance
        return $"{Environment.MachineName}-{Process.GetCurrentProcess().Id}-{Guid.NewGuid():N}";
    }
}
```

#### Vertical Scaling (More Threads per Worker)

```csharp
public class JobWorkerPool
{
    private readonly int _maxConcurrentJobs = 10; // Per worker
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var tasks = Enumerable.Range(0, _maxConcurrentJobs)
            .Select(_ => WorkerThreadAsync(stoppingToken))
            .ToArray();
            
        await Task.WhenAll(tasks);
    }
    
    private async Task WorkerThreadAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var job = await ClaimNextJobAsync();
            if (job != null)
            {
                await ExecuteJobAsync(job);
            }
            else
            {
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}
```

---

### Failure Scenarios & Recovery

#### Scenario 1: Worker Crashes During Execution

```
Time 00:00 - Worker A claims Job 1, starts execution
Time 00:05 - Worker A crashes (server restart, OOM, etc.)
Time 00:06 - Job 1 still showing "Running" in database
Time 00:11 - Heartbeat expires (NOW() > lock_expires_at)
Time 00:12 - OrphanedJobReclaimer detects expired lock
Time 00:13 - Job 1 reset to "Pending" (retry_count++)
Time 00:14 - Worker B claims Job 1, retries execution
Time 00:15 - Job 1 completes successfully ✅
```

#### Scenario 2: Database Connection Lost

```csharp
public async Task<JobExecution?> ClaimNextJobAsync()
{
    int retryCount = 0;
    while (retryCount < 3)
    {
        try
        {
            return await ClaimJobWithLockAsync();
        }
        catch (NpgsqlException ex) when (ex.IsTransient)
        {
            retryCount++;
            await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, retryCount)));
        }
    }
    
    return null; // Give up, try again in next poll cycle
}
```

#### Scenario 3: Job Times Out

```sql
-- Mark timed-out jobs
UPDATE JOB_EXECUTION
SET status = 'TimedOut',
    completed_at = NOW(),
    error_message = 'Job exceeded timeout of ' || 
      (SELECT timeout_seconds FROM JOB_DEFINITION WHERE id = job_definition_id) || ' seconds'
WHERE status = 'Running'
  AND started_at < NOW() - (
    SELECT INTERVAL '1 second' * timeout_seconds 
    FROM JOB_DEFINITION 
    WHERE id = job_definition_id
  );
```

---

### Distributed Lock Comparison

| Approach | Pros | Cons | When to Use |
|----------|------|------|-------------|
| **PostgreSQL Row Lock** | ✅ No extra dependencies<br>✅ ACID guarantees<br>✅ Simple | ❌ Database load<br>❌ Requires polling | Single region, <10K jobs/min |
| **Redis SETNX** | ✅ Very fast<br>✅ Low latency<br>✅ TTL built-in | ❌ Extra dependency<br>❌ No ACID<br>❌ Split brain risk | High throughput (>10K jobs/min) |
| **Hangfire** | ✅ Built-in lock<br>✅ Battle-tested | ❌ Black box<br>❌ Less control | Don't want to manage locks |
| **Distributed Lock Service** (etcd, Consul) | ✅ Purpose-built<br>✅ Highly available | ❌ Operational complexity | Multi-region, mission-critical |

**Recommendation for ContentOS:**
- **Start with PostgreSQL row locks** (simplest, no extra dependencies)
- **Add Redis locks** if throughput >10K jobs/minute
- **Use Hangfire** if you want managed solution

### IBackgroundJobService

Located in: `TechWayFit.ContentOS.Abstractions`

```csharp
namespace TechWayFit.ContentOS.Abstractions.Jobs;

/// <summary>
/// Service for enqueueing and managing background jobs
/// </summary>
public interface IBackgroundJobService
{
    /// <summary>
    /// Enqueue a job to run immediately
    /// </summary>
    Task<Guid> EnqueueAsync<TJob>(
        object? parameters = null,
        int priority = 0,
        CancellationToken cancellationToken = default
    ) where TJob : IBackgroundJob;
    
    /// <summary>
    /// Schedule a job to run at specific time
    /// </summary>
    Task<Guid> ScheduleAsync<TJob>(
        DateTime scheduledAt,
        object? parameters = null,
        CancellationToken cancellationToken = default
    ) where TJob : IBackgroundJob;
    
    /// <summary>
    /// Schedule a recurring job with cron expression
    /// </summary>
    Task<Guid> ScheduleRecurringAsync<TJob>(
        string cronExpression,
        object? parameters = null,
        CancellationToken cancellationToken = default
    ) where TJob : IBackgroundJob;
    
    /// <summary>
    /// Cancel a scheduled or pending job
    /// </summary>
    Task CancelAsync(Guid jobId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Retry a failed job
    /// </summary>
    Task RetryAsync(Guid jobId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Base interface for all background jobs
/// </summary>
public interface IBackgroundJob
{
    Task ExecuteAsync(object? parameters, CancellationToken cancellationToken = default);
}
```

---

## Implementation Strategies

### Option 1: Simple PostgreSQL-Based (Recommended for MVP)

**Pros:**
- ✅ No external dependencies
- ✅ Uses existing PostgreSQL
- ✅ Multi-tenant safe by design
- ✅ Transactional consistency

**Cons:**
- ❌ Limited scaling (single server)
- ❌ Manual job runner implementation

**Implementation:**
- Store jobs in `BACKGROUND_JOB` table
- Background worker polls for pending jobs
- Use `SELECT ... FOR UPDATE SKIP LOCKED` for concurrency

**Example Worker:**

```csharp
public class PostgresJobWorker : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // Poll for jobs
            var jobs = await _repository.GetPendingJobsAsync(batchSize: 10);
            
            foreach (var job in jobs)
            {
                await ExecuteJobAsync(job, stoppingToken);
            }
            
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }
}
```

---

### Option 2: Hangfire (Recommended for Production)

**Pros:**
- ✅ Battle-tested, production-ready
- ✅ Built-in UI dashboard
- ✅ Advanced retry logic, dead letter queue
- ✅ Supports PostgreSQL storage
- ✅ Distributed locks, concurrency control

**Cons:**
- ❌ External dependency (NuGet package)
- ❌ Additional learning curve

**Integration:**

```csharp
// Install: Hangfire.PostgreSql
services.AddHangfire(config =>
{
    config.UsePostgreSqlStorage(connectionString);
    config.UseFilter(new TenantJobFilter()); // Custom filter for tenant isolation
});

services.AddHangfireServer();

// Enqueue job
BackgroundJob.Enqueue<CleanupExpiredTokensJob>(x => x.ExecuteAsync(tenantId));

// Schedule recurring
RecurringJob.AddOrUpdate<GenerateReportsJob>(
    "monthly-report",
    x => x.ExecuteAsync(tenantId),
    Cron.Monthly
);
```

---

### Option 3: Azure Service Bus / RabbitMQ (For Distributed Systems)

**When to use:**
- Multiple API instances (load balancing)
- Microservices architecture
- Need guaranteed delivery

**Pros:**
- ✅ Highly scalable
- ✅ Fault-tolerant
- ✅ Message-based architecture

**Cons:**
- ❌ Additional infrastructure cost
- ❌ More complex setup

---

## Example Job Implementations

### 1. Cleanup Expired Preview Tokens

```csharp
namespace TechWayFit.ContentOS.Infrastructure.Jobs;

public class CleanupExpiredTokensJob : IBackgroundJob
{
    private readonly IPreviewTokenRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;
    
    public async Task ExecuteAsync(object? parameters, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;
        
        // Delete tokens expired >7 days ago
        var cutoffDate = DateTime.UtcNow.AddDays(-7);
        var deletedCount = await _repository.DeleteExpiredTokensAsync(cutoffDate, cancellationToken);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation(
            "Cleaned up {Count} expired tokens for tenant {TenantId}", 
            deletedCount, 
            tenantId
        );
    }
}

// Schedule in API startup:
public static void ConfigureJobs(IServiceProvider services)
{
    var jobService = services.GetRequiredService<IBackgroundJobService>();
    
    // Run daily at 2 AM
    await jobService.ScheduleRecurringAsync<CleanupExpiredTokensJob>(
        cronExpression: "0 2 * * *" // Daily at 2 AM
    );
}
```

### 2. Generate Search Index

```csharp
public class IndexContentJob : IBackgroundJob
{
    private readonly IContentRepository _contentRepository;
    private readonly ISearchIndexService _searchIndexService;
    
    public async Task ExecuteAsync(object? parameters, CancellationToken cancellationToken)
    {
        var contentId = ((dynamic)parameters).ContentId;
        
        var content = await _contentRepository.GetByIdAsync(contentId, cancellationToken);
        
        await _searchIndexService.IndexAsync(content, cancellationToken);
    }
}

// Triggered by event:
public class ContentPublishedEventHandler : IEventHandler<ContentPublishedEventV1>
{
    public async Task HandleAsync(ContentPublishedEventV1 @event, CancellationToken cancellationToken)
    {
        // Enqueue async indexing job
        await _jobService.EnqueueAsync<IndexContentJob>(
            parameters: new { ContentId = @event.ContentId },
            priority: 10 // High priority
        );
    }
}
```

### 3. Generate Embeddings for RAG

```csharp
public class GenerateEmbeddingsJob : IBackgroundJob
{
    private readonly IContentRagChunkRepository _chunkRepository;
    private readonly IEmbeddingService _embeddingService;
    private readonly IContentEmbeddingRepository _embeddingRepository;
    
    public async Task ExecuteAsync(object? parameters, CancellationToken cancellationToken)
    {
        var chunkId = ((dynamic)parameters).ChunkId;
        
        var chunk = await _chunkRepository.GetByIdAsync(chunkId, cancellationToken);
        
        // Generate embedding via OpenAI/Azure
        var embedding = await _embeddingService.GenerateAsync(
            chunk.ChunkText, 
            model: "text-embedding-3-small"
        );
        
        // Store embedding
        await _embeddingRepository.CreateAsync(new ContentEmbeddingRow
        {
            ChunkId = chunkId,
            EmbeddingVector = embedding.Vector,
            EmbeddingModel = "text-embedding-3-small",
            EmbeddingDimension = 1536,
            IndexedAt = DateTime.UtcNow
        });
    }
}
```

### 4. Export Content to CSV

```csharp
public class ExportContentJob : IBackgroundJob
{
    private readonly IContentRepository _contentRepository;
    private readonly IBlobStore _blobStore;
    private readonly INotificationService _notificationService;
    
    public async Task ExecuteAsync(object? parameters, CancellationToken cancellationToken)
    {
        var exportParams = JsonSerializer.Deserialize<ExportParameters>(parameters);
        
        // Long-running operation
        var content = await _contentRepository.GetAllAsync(
            exportParams.Filters, 
            cancellationToken
        );
        
        var csvData = GenerateCsv(content);
        
        // Upload to blob storage
        var blobPath = await _blobStore.UploadAsync(
            $"exports/{Guid.NewGuid()}.csv", 
            csvData
        );
        
        // Notify user
        await _notificationService.SendAsync(
            exportParams.UserId,
            "Export Complete",
            $"Your export is ready: {blobPath}"
        );
    }
}
```

---

## Common Job Patterns

### Pattern 1: Instance-Scoped Jobs (Run on Every Server)

#### Example 1: Clear Local Memory Cache

```csharp
public class ClearLocalCacheJob : IBackgroundJob
{
    private readonly IMemoryCache _cache;
    
    public async Task ExecuteAsync(object? parameters, CancellationToken cancellationToken)
    {
        // Clear this server's in-memory cache
        _cache.Clear();
        
        _logger.LogInformation(
            "Cleared local cache on worker {WorkerId}", 
            Environment.MachineName
        );
    }
}

// Register as instance-scoped (runs on all servers)
await jobService.ScheduleRecurringAsync<ClearLocalCacheJob>(
    cronExpression: "0 */6 * * *",  // Every 6 hours
    parameters: new { ExecutionScope = "Instance" }
);
```

#### Example 2: Server Health Check

```csharp
public class ServerHealthCheckJob : IBackgroundJob
{
    private readonly IMetricsCollector _metrics;
    
    public async Task ExecuteAsync(object? parameters, CancellationToken cancellationToken)
    {
        var health = new
        {
            ServerId = Environment.MachineName,
            CpuUsage = await GetCpuUsageAsync(),
            MemoryUsage = GC.GetTotalMemory(false),
            ActiveConnections = await GetActiveConnectionsAsync(),
            CheckedAt = DateTime.UtcNow
        };
        
        // Report metrics for THIS server only
        await _metrics.ReportHealthAsync(health);
    }
}
```

#### Example 3: Cleanup Local Temp Files

```csharp
public class CleanupLocalTempFilesJob : IBackgroundJob
{
    public async Task ExecuteAsync(object? parameters, CancellationToken cancellationToken)
    {
        var tempPath = Path.Combine(Path.GetTempPath(), "contentos");
        
        if (Directory.Exists(tempPath))
        {
            var files = Directory.GetFiles(tempPath)
                .Where(f => File.GetCreationTime(f) < DateTime.UtcNow.AddDays(-1));
            
            foreach (var file in files)
            {
                File.Delete(file);
            }
            
            _logger.LogInformation(
                "Cleaned {Count} temp files on {Server}", 
                files.Count(), 
                Environment.MachineName
            );
        }
    }
}
```

---

### Pattern 2: Cluster-Scoped Jobs (Run Once Across Farm)

#### Example: Send Scheduled Email (must run once)

```csharp
public class SendScheduledEmailJob : IBackgroundJob
{
    private readonly IEmailService _emailService;
    
    public async Task ExecuteAsync(object? parameters, CancellationToken cancellationToken)
    {
        var emailParams = JsonSerializer.Deserialize<EmailParameters>(parameters);
        
        // Only ONE server executes this (via distributed lock)
        await _emailService.SendAsync(emailParams.To, emailParams.Subject, emailParams.Body);
        
        _logger.LogInformation("Sent email to {To} from worker {Worker}", 
            emailParams.To, 
            Environment.MachineName
        );
    }
}

// Register as cluster-scoped (default)
await jobService.ScheduleRecurringAsync<SendScheduledEmailJob>(
    cronExpression: "0 9 * * *",  // Daily at 9 AM
    parameters: new { ExecutionScope = "Cluster" }  // Explicit (or omit, it's default)
);
```

---

### Pattern 3: Code-Based Job Chaining (Recommended)

**Use case:** Sequential workflow where Job A triggers Job B on success.

```csharp
public class ProcessContentJob : IBackgroundJob
{
    private readonly IBackgroundJobService _jobService;
    private readonly IContentProcessor _processor;
    
    public async Task ExecuteAsync(object? parameters, CancellationToken cancellationToken)
    {
        var contentId = ((dynamic)parameters).ContentId;
        
        // Step 1: Process content
        var processedContent = await _processor.ProcessAsync(contentId);
        
        // Step 2: Explicitly trigger next job in chain
        await _jobService.EnqueueAsync<GenerateEmbeddingsJob>(
            parameters: new { ContentId = contentId },
            priority: 10
        );
        
        _logger.LogInformation(
            "Processed content {ContentId}, enqueued embedding generation",
            contentId
        );
    }
}
```

**Benefits:**
- ✅ Explicit workflow - easy to understand by reading code
- ✅ Conditional chaining - `if (shouldIndex) await EnqueueAsync<IndexJob>()`
- ✅ Pass data between jobs via parameters
- ✅ No complex dependency graph in database

**Transaction Safety:**
```csharp
// Ensure job completion + next job enqueue are atomic
using var transaction = await _unitOfWork.BeginTransactionAsync();
try
{
    await DoWorkAsync();
    await _jobService.EnqueueAsync<NextJob>(...);
    await transaction.CommitAsync();
}
catch
{
    await transaction.RollbackAsync();
    throw; // Retry this job, next job NOT enqueued
}
```

---

### Pattern 4: Manual/On-Demand Jobs

**Use case:** Jobs triggered via API or by other jobs, no automatic schedule.

```csharp
// Job definition (schedule_type = 'Manual')
public class ExportContentJob : IBackgroundJob
{
    public async Task ExecuteAsync(object? parameters, CancellationToken cancellationToken)
    {
        var exportParams = JsonSerializer.Deserialize<ExportRequest>(parameters);
        
        var content = await _repository.GetByFilterAsync(exportParams.Filters);
        var csvData = GenerateCsv(content);
        
        await _blobStore.UploadAsync($"exports/{exportParams.ExportId}.csv", csvData);
    }
}

// API endpoint to trigger manually
[HttpPost("api/content/export")]
public async Task<IActionResult> ExportContent([FromBody] ExportRequest request)
{
    // Enqueue job on-demand
    var jobId = await _jobService.EnqueueAsync<ExportContentJob>(
        parameters: request,
        priority: 5
    );
    
    return Accepted(new { JobId = jobId, Status = "Processing" });
}

// Or triggered by another job
public class MonthlyReportJob : IBackgroundJob
{
    public async Task ExecuteAsync(object? parameters, CancellationToken cancellationToken)
    {
        // Generate report data
        var reportData = await GenerateReportAsync();
        
        // Trigger export job manually
        await _jobService.EnqueueAsync<ExportContentJob>(
            parameters: new ExportRequest { Data = reportData }
        );
    }
}
```

**Register as manual job (no schedule):**
```sql
INSERT INTO JOB_DEFINITION (
  tenant_id, job_name, job_key, job_type,
  schedule_type, is_enabled
)
VALUES (
  :tenant_id, 'ExportContent', 'export-content',
  'TechWayFit.ContentOS.Infrastructure.Jobs.ExportContentJob',
  'Manual',  -- ← No automatic schedule
  true
);
```

---

### Pattern 5: Retry with Exponential Backoff

```csharp
public class ResilientJob : IBackgroundJob
{
    public async Task ExecuteAsync(object? parameters, CancellationToken cancellationToken)
    {
        try
        {
            // Perform work
            await DoWorkAsync();
        }
        catch (TransientException ex)
        {
            // Will retry automatically based on max_retries
            throw; // Let job runner handle retry
        }
        catch (PermanentException ex)
        {
            // Log and mark as failed (no retry)
            _logger.LogError(ex, "Permanent failure");
            throw new JobPermanentFailureException(ex.Message);
        }
    }
}
```

### Pattern 2: Long-Running with Progress Updates

```csharp
public class BulkProcessingJob : IBackgroundJob
{
    public async Task ExecuteAsync(object? parameters, CancellationToken cancellationToken)
    {
        var items = await GetItemsToProcessAsync();
        
        for (int i = 0; i < items.Count; i++)
        {
            await ProcessItemAsync(items[i]);
            
            // Update progress (store in job.result_data)
            await UpdateProgressAsync(
                jobId: parameters.JobId,
                progress: (i + 1) * 100 / items.Count,
                message: $"Processed {i + 1}/{items.Count}"
            );
        }
    }
}
```

### Pattern 7: Tenant-Scoped Recurring Job

```csharp
// Generate monthly report for each tenant
public class MonthlyReportJob : IBackgroundJob
{
    public async Task ExecuteAsync(object? parameters, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;
        
        var data = await _reportingService.GenerateMonthlyDataAsync(
            tenantId, 
            DateTime.UtcNow.AddMonths(-1)
        );
        
        var report = await _reportGenerator.CreatePdfAsync(data);
        
        // Store in blob storage
        var path = $"{tenantId}/reports/{DateTime.UtcNow:yyyy-MM}.pdf";
        await _blobStore.UploadAsync(path, report);
    }
}

// Register per tenant (in tenant onboarding)
public async Task OnTenantCreated(Guid tenantId)
{
    await _jobService.ScheduleRecurringAsync<MonthlyReportJob>(
        cronExpression: "0 0 1 * *", // First day of month at midnight
        parameters: new { TenantId = tenantId }
    );
}
```

---

## Job Lifecycle (Improved)

```
┌────────────────────┐
│ JOB_DEFINITION     │  ← Created once, defines the job
│ (metadata)         │
│ - What to run      │
│ - When to run      │
│ - How to retry     │
└─────────┬──────────┘
          │
          ↓ (Scheduler creates execution)
┌─────────────────────┐
│ JOB_EXECUTION       │  ← New row per execution
│ status: Pending     │
└─────────┬───────────┘
          │
          ↓ (Worker claims via SKIP LOCKED)
┌─────────────────────┐
│ JOB_EXECUTION       │
│ status: Claimed     │  ← Row-level lock acquired
│ claimed_by: worker-1│
│ lock_expires_at: +5m│
└─────────┬───────────┘
          │
          ↓ (Worker starts execution)
┌─────────────────────┐
│ JOB_EXECUTION       │
│ status: Running     │  ← Heartbeat updates every 30s
│ heartbeat_at: NOW() │
└─────────┬───────────┘
          │
          ├──────────────────┬──────────────────┬─────────────────┐
          ↓                  ↓                  ↓                 ↓
    ┌──────────┐      ┌──────────┐      ┌──────────┐     ┌──────────┐
    │Completed │      │  Failed  │      │TimedOut  │     │Cancelled │
    └────┬─────┘      └────┬─────┘      └────┬─────┘     └──────────┘
         │                 │                  │
         │                 ↓ (retry?)         │
         │          ┌─────────────┐           │
         │          │ New         │           │
         │          │ JOB_EXECUTION│          │
         │          │ is_retry=true│          │
         │          └──────┬──────┘           │
         │                 │                  │
         │                 ↓ Pending          │
         │                                    │
         └──────────────┬───────────────────┘
                        ↓
              ┌──────────────────────┐
              │ JOB_EXECUTION_HISTORY│  ← Archived after 30 days
              │ (long-term storage)  │
              └──────────────────────┘

For Recurring Jobs:
┌────────────────────┐
│ JOB_DEFINITION     │
│ cron: "0 * * * *"  │  ← Runs every hour
└─────────┬──────────┘
          │
          ├──→ Execution 1 (09:00) → Completed
          ├──→ Execution 2 (10:00) → Completed
          ├──→ Execution 3 (11:00) → Running
          └──→ Execution 4 (12:00) → Pending (future)
```

### State Transitions

```
Pending ──┬──→ Claimed ──→ Running ──┬──→ Completed
          │                          │
          │                          ├──→ Failed ──→ (retry) → Pending
          │                          │
          │                          └──→ TimedOut ──→ (retry) → Pending
          │
          └──→ Cancelled

Orphaned (heartbeat expired):
  Running (lock expired) ──→ Pending (auto-recovery)
```

---

## Monitoring & Observability

### Metrics to Track

1. **Job Execution**
   - Jobs completed/failed per minute
   - Average execution duration (by job type)
   - Queue depth (pending executions)
   - Execution lag (scheduled_at vs. actual started_at)

2. **Health Indicators**
   - Orphaned jobs (lock expired, not reclaimed)
   - Jobs stuck in "Running" > timeout
   - Failed jobs not retrying
   - Dead letter queue size (max retries exceeded)
   - Worker heartbeat gaps

3. **Tenant-Specific**
   - Jobs per tenant
   - Failed job rate by tenant
   - Average execution time by tenant

4. **Web Farm Metrics**
   - Active workers count
   - Jobs claimed per worker
   - Worker claim success rate
   - Lock contention (jobs skipped due to locks)

### Dashboard Queries

```sql
-- Current queue depth (pending executions)
SELECT COUNT(*) AS pending_jobs
FROM JOB_EXECUTION 
WHERE status = 'Pending' AND scheduled_at <= NOW();

-- Active workers (claimed jobs in last 5 minutes)
SELECT claimed_by, COUNT(*) AS active_jobs
FROM JOB_EXECUTION
WHERE status IN ('Claimed', 'Running')
  AND claimed_at > NOW() - INTERVAL '5 minutes'
GROUP BY claimed_by;

-- Orphaned jobs (heartbeat expired)
SELECT je.id, je.claimed_by, jd.job_name,
       NOW() - je.heartbeat_at AS time_since_heartbeat
FROM JOB_EXECUTION je
JOIN JOB_DEFINITION jd ON je.job_definition_id = jd.id
WHERE je.status IN ('Claimed', 'Running')
  AND je.lock_expires_at < NOW();

-- Execution lag (jobs running behind schedule)
SELECT jd.job_name, 
       AVG(EXTRACT(EPOCH FROM (je.started_at - je.scheduled_at))) AS avg_lag_seconds,
       COUNT(*) AS execution_count
FROM JOB_EXECUTION je
JOIN JOB_DEFINITION jd ON je.job_definition_id = jd.id
WHERE je.started_at IS NOT NULL
  AND je.created_on > NOW() - INTERVAL '1 hour'
GROUP BY jd.job_name
HAVING AVG(EXTRACT(EPOCH FROM (je.started_at - je.scheduled_at))) > 60
ORDER BY avg_lag_seconds DESC;

-- Failed jobs by type (last 24h)
SELECT jd.job_type, 
       COUNT(*) AS failures,
       COUNT(DISTINCT je.tenant_id) AS affected_tenants
FROM JOB_EXECUTION je
JOIN JOB_DEFINITION jd ON je.job_definition_id = jd.id
WHERE je.status = 'Failed' 
  AND je.created_on > NOW() - INTERVAL '24 hours'
GROUP BY jd.job_type
ORDER BY failures DESC;

-- Jobs exceeding timeout
SELECT je.id, jd.job_name, je.claimed_by,
       EXTRACT(EPOCH FROM (NOW() - je.started_at)) AS running_seconds,
       jd.timeout_seconds
FROM JOB_EXECUTION je
JOIN JOB_DEFINITION jd ON je.job_definition_id = jd.id
WHERE je.status = 'Running'
  AND je.started_at < NOW() - (INTERVAL '1 second' * jd.timeout_seconds);

-- Worker performance (last hour)
SELECT 
  je.claimed_by AS worker,
  COUNT(*) AS total_jobs,
  COUNT(*) FILTER (WHERE je.status = 'Completed') AS completed,
  COUNT(*) FILTER (WHERE je.status = 'Failed') AS failed,
  AVG(je.duration_ms) FILTER (WHERE je.status = 'Completed') AS avg_duration_ms,
  MAX(je.duration_ms) FILTER (WHERE je.status = 'Completed') AS max_duration_ms
FROM JOB_EXECUTION je
WHERE je.claimed_at > NOW() - INTERVAL '1 hour'
  AND je.claimed_by IS NOT NULL
GROUP BY je.claimed_by
ORDER BY total_jobs DESC;

-- Retry analysis
SELECT jd.job_name,
       AVG(je.retry_count) AS avg_retries,
       MAX(je.retry_count) AS max_retries,
       COUNT(*) FILTER (WHERE je.retry_count > 0) AS jobs_with_retries
FROM JOB_EXECUTION je
JOIN JOB_DEFINITION jd ON je.job_definition_id = jd.id
WHERE je.created_on > NOW() - INTERVAL '24 hours'
GROUP BY jd.job_name
HAVING COUNT(*) FILTER (WHERE je.retry_count > 0) > 0
ORDER BY avg_retries DESC;
```

---

## Recommended Approach for ContentOS

### Phase 1: MVP (Simple PostgreSQL)
- ✅ Implement `BACKGROUND_JOB` table
- ✅ Create `IBackgroundJobService` interface
- ✅ Build simple polling worker with `BackgroundService`
- ✅ Support enqueue, schedule, recurring jobs

**Use cases:**
- Cleanup expired tokens
- Index content after publish
- Generate embeddings

### Phase 2: Production (Hangfire)
- ✅ Add Hangfire.PostgreSql
- ✅ Swap implementation to `HangfireBackgroundJobService`
- ✅ Enable dashboard for monitoring
- ✅ Configure advanced retry policies

**Use cases:**
- All MVP jobs + distributed execution
- Long-running exports
- Tenant-specific recurring reports

### Phase 3: Scale (Message Queues)
- ✅ Add Azure Service Bus / RabbitMQ
- ✅ Separate job workers into dedicated services
- ✅ Implement event-driven job triggers

**Use cases:**
- High-volume processing
- Multi-region deployments
- Microservices architecture

---

## Job Workflow Design Summary

### Recommended Patterns for ContentOS

| Pattern | When to Use | Implementation |
|---------|-------------|----------------|
| **Scheduled (Recurring)** | Cleanup, reports, maintenance | `schedule_type: 'Cron'` + cron expression |
| **Event-Driven** | After domain events | Event handler → `EnqueueAsync` |
| **Code-Based Chaining** | Sequential workflows | Job A → `EnqueueAsync<JobB>()` on success |
| **Manual/On-Demand** | API-triggered, ad-hoc tasks | `schedule_type: 'Manual'` + API endpoint |
| **Instance-Scoped** | Server-local tasks | `execution_scope: 'Instance'` |

### Decision Tree

```
Need to run job?
│
├─ Automatically on schedule?
│  ├─ YES → Use schedule_type: 'Cron' or 'Recurring'
│  └─ NO → Continue
│
├─ Triggered by domain event?
│  ├─ YES → Event handler + EnqueueAsync
│  └─ NO → Continue
│
├─ Part of multi-step workflow?
│  ├─ YES → Code-based chaining (Job A enqueues Job B)
│  └─ NO → Continue
│
└─ On-demand via API?
   └─ YES → schedule_type: 'Manual' + API endpoint
```

### Example: Content Publishing Workflow

```csharp
// 1. Event-driven trigger
public class ContentPublishedEventHandler : IEventHandler<ContentPublishedEventV1>
{
    public async Task HandleAsync(ContentPublishedEventV1 @event)
    {
        // Start workflow by enqueueing first job
        await _jobService.EnqueueAsync<ProcessContentJob>(
            parameters: new { ContentId = @event.ContentId }
        );
    }
}

// 2. Job chains to next step
public class ProcessContentJob : IBackgroundJob
{
    public async Task ExecuteAsync(object? parameters, CancellationToken cancellationToken)
    {
        var contentId = ((dynamic)parameters).ContentId;
        await _processor.ProcessAsync(contentId);
        
        // Explicit chain to embedding generation
        await _jobService.EnqueueAsync<GenerateEmbeddingsJob>(
            parameters: new { ContentId = contentId }
        );
    }
}

// 3. Final job in chain
public class GenerateEmbeddingsJob : IBackgroundJob
{
    public async Task ExecuteAsync(object? parameters, CancellationToken cancellationToken)
    {
        var contentId = ((dynamic)parameters).ContentId;
        await _embeddingService.GenerateAsync(contentId);
        
        // Optionally trigger indexing
        await _jobService.EnqueueAsync<IndexContentJob>(
            parameters: new { ContentId = contentId }
        );
    }
}

// Result: Event → Process → Embeddings → Index (all explicit, all traceable)
```

---

## Next Steps

1. **Create database schema** - Add JOB_DEFINITION, JOB_EXECUTION, JOB_EXECUTION_HISTORY tables
2. **Define abstractions** - IBackgroundJobService, IBackgroundJob interfaces
3. **Implement MVP** - PostgreSQL-based job runner with SKIP LOCKED
4. **Create example jobs** - CleanupExpiredTokensJob, IndexContentJob, GenerateEmbeddingsJob
5. **Add monitoring** - Job execution metrics and dashboard
6. **Expose API** - Manual job triggering endpoints

Ready to implement when you are!
