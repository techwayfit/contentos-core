using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Jobs;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Configurations.Jobs;

public class JobDefinitionConfiguration : IEntityTypeConfiguration<JobDefinitionRow>
{
    public void Configure(EntityTypeBuilder<JobDefinitionRow> builder)
    {
        builder.ToTable("job_definition");
        
        // Primary key
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        
        // Tenant key
        builder.ConfigureTenantKey();
        
        // Job identification
        builder.Property(x => x.JobName)
            .HasColumnName("job_name")
            .HasMaxLength(200)
            .IsRequired();
        
        builder.Property(x => x.JobKey)
            .HasColumnName("job_key")
            .HasMaxLength(200)
            .IsRequired();
        
        builder.Property(x => x.JobType)
            .HasColumnName("job_type")
            .HasMaxLength(500)
            .IsRequired();
        
        builder.Property(x => x.JobParameters)
            .HasColumnName("job_parameters")
            .HasColumnType("jsonb");
        
        // Scheduling configuration
        builder.Property(x => x.ScheduleType)
            .HasColumnName("schedule_type")
            .HasMaxLength(50)
            .IsRequired();
        
        builder.Property(x => x.CronExpression)
            .HasColumnName("cron_expression")
            .HasMaxLength(100);
        
        builder.Property(x => x.IntervalSeconds)
            .HasColumnName("interval_seconds");
        
        // Execution settings
        builder.Property(x => x.Priority)
            .HasColumnName("priority")
            .HasDefaultValue(0);
        
        builder.Property(x => x.ExecutionScope)
            .HasColumnName("execution_scope")
            .HasMaxLength(50)
            .HasDefaultValue("Cluster");
        
        builder.Property(x => x.MaxRetries)
            .HasColumnName("max_retries")
            .HasDefaultValue(3);
        
        builder.Property(x => x.TimeoutSeconds)
            .HasColumnName("timeout_seconds")
            .HasDefaultValue(3600);
        
        builder.Property(x => x.MaxConcurrentExecutions)
            .HasColumnName("max_concurrent_executions")
            .HasDefaultValue(1);
        
        // State
        builder.Property(x => x.IsEnabled)
            .HasColumnName("is_enabled")
            .HasDefaultValue(true);
        
        builder.Property(x => x.LastExecutionId)
            .HasColumnName("last_execution_id");
        
        builder.Property(x => x.NextRunAt)
            .HasColumnName("next_run_at")
            .HasColumnType("timestamp without time zone");
        
        // Audit fields
        builder.ConfigureAuditFields();
        
        // Relationships
        builder.HasOne(x => x.LastExecution)
            .WithMany()
            .HasForeignKey(x => x.LastExecutionId)
            .OnDelete(DeleteBehavior.SetNull);
        
        builder.HasMany(x => x.Executions)
            .WithOne(x => x.JobDefinition)
            .HasForeignKey(x => x.JobDefinitionId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Constraints
        builder.HasIndex(x => new { x.TenantId, x.JobKey })
            .IsUnique()
            .HasFilter("deleted_on IS NULL")
            .HasDatabaseName("idx_job_def_tenant_key");
        
        // Indexes
        builder.HasIndex(x => new { x.TenantId, x.IsEnabled, x.NextRunAt })
            .HasFilter("is_enabled = true AND is_deleted = false")
            .HasDatabaseName("idx_job_def_next_run");
        
        builder.HasIndex(x => new { x.TenantId, x.JobType })
            .HasDatabaseName("idx_job_def_type");
        
        builder.HasIndex(x => new { x.TenantId, x.JobKey })
            .HasDatabaseName("idx_job_def_key");
    }
}
