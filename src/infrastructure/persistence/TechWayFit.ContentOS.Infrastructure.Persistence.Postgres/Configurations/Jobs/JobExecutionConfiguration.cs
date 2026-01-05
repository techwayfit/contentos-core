using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Jobs;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Configurations.Jobs;

public class JobExecutionConfiguration : IEntityTypeConfiguration<JobExecutionRow>
{
    public void Configure(EntityTypeBuilder<JobExecutionRow> builder)
    {
        builder.ToTable("job_execution");
        
        // Primary key
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        
        // Tenant key
        builder.ConfigureTenantKey();
        
        // Job definition reference
        builder.Property(x => x.JobDefinitionId)
            .HasColumnName("job_definition_id")
            .IsRequired();
        
        // Execution context
        builder.Property(x => x.ExecutionNumber)
            .HasColumnName("execution_number")
            .IsRequired();
        
        builder.Property(x => x.ScheduledAt)
            .HasColumnName("scheduled_at")
            .HasColumnType("timestamp without time zone")
            .IsRequired();
        
        builder.Property(x => x.EnqueuedAt)
            .HasColumnName("enqueued_at")
            .HasColumnType("timestamp without time zone")
            .IsRequired();
        
        // Distributed lock (web farm support)
        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasMaxLength(50)
            .IsRequired();
        
        builder.Property(x => x.ClaimedBy)
            .HasColumnName("claimed_by")
            .HasMaxLength(200);
        
        builder.Property(x => x.ClaimedAt)
            .HasColumnName("claimed_at")
            .HasColumnType("timestamp without time zone");
        
        builder.Property(x => x.LockExpiresAt)
            .HasColumnName("lock_expires_at")
            .HasColumnType("timestamp without time zone");
        
        builder.Property(x => x.HeartbeatAt)
            .HasColumnName("heartbeat_at")
            .HasColumnType("timestamp without time zone");
        
        // Execution tracking
        builder.Property(x => x.StartedAt)
            .HasColumnName("started_at")
            .HasColumnType("timestamp without time zone");
        
        builder.Property(x => x.CompletedAt)
            .HasColumnName("completed_at")
            .HasColumnType("timestamp without time zone");
        
        builder.Property(x => x.DurationMs)
            .HasColumnName("duration_ms");
        
        // Results
        builder.Property(x => x.ResultData)
            .HasColumnName("result_data")
            .HasColumnType("jsonb");
        
        builder.Property(x => x.ErrorMessage)
            .HasColumnName("error_message")
            .HasColumnType("text");
        
        builder.Property(x => x.ErrorType)
            .HasColumnName("error_type")
            .HasMaxLength(200);
        
        builder.Property(x => x.StackTrace)
            .HasColumnName("stack_trace")
            .HasColumnType("text");
        
        // Retry tracking
        builder.Property(x => x.IsRetry)
            .HasColumnName("is_retry")
            .HasDefaultValue(false);
        
        builder.Property(x => x.RetryOfExecutionId)
            .HasColumnName("retry_of_execution_id");
        
        builder.Property(x => x.RetryCount)
            .HasColumnName("retry_count")
            .HasDefaultValue(0);
        
        builder.Property(x => x.CanRetry)
            .HasColumnName("can_retry")
            .HasDefaultValue(true);
        
        // Metadata
        builder.Property(x => x.CreatedOn)
            .HasColumnName("created_on")
            .HasColumnType("timestamp without time zone")
            .IsRequired();
        
        // Relationships
        builder.HasOne(x => x.JobDefinition)
            .WithMany(x => x.Executions)
            .HasForeignKey(x => x.JobDefinitionId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(x => x.RetryOfExecution)
            .WithMany(x => x.Retries)
            .HasForeignKey(x => x.RetryOfExecutionId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Indexes
        builder.HasIndex(x => new { x.TenantId, x.Status, x.ScheduledAt })
            .HasFilter("status = 'Pending'")
            .HasDatabaseName("idx_job_exec_pending");
        
        builder.HasIndex(x => new { x.ClaimedBy, x.Status, x.LockExpiresAt })
            .HasFilter("status IN ('Claimed', 'Running')")
            .HasDatabaseName("idx_job_exec_claimed");
        
        builder.HasIndex(x => new { x.Status, x.LockExpiresAt })
            .HasFilter("status IN ('Claimed', 'Running')")
            .HasDatabaseName("idx_job_exec_orphaned");
        
        builder.HasIndex(x => new { x.JobDefinitionId, x.CreatedOn })
            .IsDescending(false, true)
            .HasDatabaseName("idx_job_exec_definition");
        
        builder.HasIndex(x => x.RetryOfExecutionId)
            .HasDatabaseName("idx_job_exec_retry");
    }
}
