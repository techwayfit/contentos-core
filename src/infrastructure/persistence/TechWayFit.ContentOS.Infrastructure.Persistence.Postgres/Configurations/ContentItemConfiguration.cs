using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Entities;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Configurations;

/// <summary>
/// EF Core configuration for ContentItemRow
/// </summary>
public class ContentItemConfiguration : IEntityTypeConfiguration<ContentItemRow>
{
    public void Configure(EntityTypeBuilder<ContentItemRow> builder)
    {
        builder.ToTable("content_items");

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

        // Content metadata
        builder.Property(x => x.ContentType)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("content_type");

        builder.Property(x => x.DefaultLanguage)
            .IsRequired()
            .HasMaxLength(10)
            .HasColumnName("default_language");

        builder.Property(x => x.WorkflowStatus)
            .IsRequired()
            .HasColumnName("workflow_status");

        // Audit fields
        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");

        builder.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(x => x.CreatedBy)
            .HasColumnName("created_by");

        builder.Property(x => x.UpdatedBy)
            .HasColumnName("updated_by");

        // Multi-tenant composite index (CRITICAL for query performance)
        builder.HasIndex(x => new { x.TenantId, x.SiteId, x.Environment })
            .HasDatabaseName("ix_content_items_tenant");

        // Additional indexes
        builder.HasIndex(x => x.ContentType)
            .HasDatabaseName("ix_content_items_content_type");

        builder.HasIndex(x => x.WorkflowStatus)
            .HasDatabaseName("ix_content_items_workflow_status");

        builder.HasIndex(x => x.CreatedAt)
            .HasDatabaseName("ix_content_items_created_at");

        // Relationships
        builder.HasMany(x => x.Localizations)
            .WithOne(x => x.ContentItem)
            .HasForeignKey(x => x.ContentItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
