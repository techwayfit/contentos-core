using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Entities;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Configurations;

/// <summary>
/// EF Core configuration for WorkflowStateRow
/// </summary>
public class WorkflowStateConfiguration : IEntityTypeConfiguration<WorkflowStateRow>
{
    public void Configure(EntityTypeBuilder<WorkflowStateRow> builder)
    {
        builder.ToTable("workflow_states");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        // Multi-tenant fields
        builder.Property(x => x.TenantId)
            .IsRequired()
            .HasColumnName("tenant_id");

        builder.Property(x => x.SiteId)
            .IsRequired()
            .HasColumnName("site_id");

        builder.Property(x => x.Environment)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnName("environment");

        builder.Property(x => x.ContentItemId)
            .IsRequired()
            .HasColumnName("content_item_id");

        builder.Property(x => x.CurrentStatus)
            .IsRequired()
            .HasColumnName("current_status");

        builder.Property(x => x.PreviousStatus)
            .HasColumnName("previous_status");

        builder.Property(x => x.TransitionedBy)
            .HasColumnName("transitioned_by");

        builder.Property(x => x.TransitionedAt)
            .IsRequired()
            .HasColumnName("transitioned_at");

        builder.Property(x => x.Comment)
            .HasMaxLength(1000)
            .HasColumnName("comment");

        // Multi-tenant composite index (CRITICAL for query performance)
        builder.HasIndex(x => new { x.TenantId, x.SiteId, x.Environment })
            .HasDatabaseName("ix_workflow_states_tenant");

        // Index for content item lookups (now includes tenant for better performance)
        builder.HasIndex(x => new { x.TenantId, x.ContentItemId })
            .IsUnique()
            .HasDatabaseName("ix_workflow_states_content_item");

        // Index for status queries
        builder.HasIndex(x => x.CurrentStatus)
            .HasDatabaseName("ix_workflow_states_current_status");
    }
}
