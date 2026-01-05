using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Jobs;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Configurations.Jobs;

public class JobExecutionHistoryConfiguration : IEntityTypeConfiguration<JobExecutionHistoryRow>
{
    public void Configure(EntityTypeBuilder<JobExecutionHistoryRow> builder)
    {
        builder.ToTable("job_execution_history");
        
        // Primary key
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        
        // Tenant reference (no FK, denormalized for history)
        builder.Property(x => x.TenantId)
            .HasColumnName("tenant_id")
            .IsRequired();
        
        builder.Property(x => x.JobDefinitionId)
            .HasColumnName("job_definition_id")
            .IsRequired();
        
        // Denormalized fields
        builder.Property(x => x.JobName)
            .HasColumnName("job_name")
            .HasMaxLength(200)
            .IsRequired();
        
        builder.Property(x => x.JobType)
            .HasColumnName("job_type")
            .HasMaxLength(500)
            .IsRequired();
        
        // Execution data
        builder.Property(x => x.ExecutionNumber)
            .HasColumnName("execution_number");
        
        builder.Property(x => x.ScheduledAt)
            .HasColumnName("scheduled_at")
            .HasColumnType("timestamp without time zone");
        
        builder.Property(x => x.StartedAt)
            .HasColumnName("started_at")
            .HasColumnType("timestamp without time zone");
        
        builder.Property(x => x.CompletedAt)
            .HasColumnName("completed_at")
            .HasColumnType("timestamp without time zone");
        
        builder.Property(x => x.DurationMs)
            .HasColumnName("duration_ms");
        
        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasMaxLength(50)
            .IsRequired();
        
        builder.Property(x => x.ClaimedBy)
            .HasColumnName("claimed_by")
            .HasMaxLength(200);
        
        builder.Property(x => x.ResultData)
            .HasColumnName("result_data")
            .HasColumnType("jsonb");
        
        builder.Property(x => x.ErrorMessage)
            .HasColumnName("error_message")
            .HasColumnType("text");
        
        builder.Property(x => x.ErrorType)
            .HasColumnName("error_type")
            .HasMaxLength(200);
        
        builder.Property(x => x.RetryCount)
            .HasColumnName("retry_count");
        
        builder.Property(x => x.ArchivedAt)
            .HasColumnName("archived_at")
            .HasColumnType("timestamp without time zone")
            .IsRequired();
        
        // Indexes
        builder.HasIndex(x => new { x.TenantId, x.ArchivedAt })
            .IsDescending(false, true)
            .HasDatabaseName("idx_job_history_tenant_date");
        
        builder.HasIndex(x => new { x.JobDefinitionId, x.ArchivedAt })
            .IsDescending(false, true)
            .HasDatabaseName("idx_job_history_definition");
        
        builder.HasIndex(x => new { x.TenantId, x.Status, x.ArchivedAt })
            .IsDescending(false, false, true)
            .HasDatabaseName("idx_job_history_status");
        
        // TODO: Implement partitioning in migration
        // PARTITION BY RANGE (archived_at)
        // Monthly partitions for scalability
    }
}
