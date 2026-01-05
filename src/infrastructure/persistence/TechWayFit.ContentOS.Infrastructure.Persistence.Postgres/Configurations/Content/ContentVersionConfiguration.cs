using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Content;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Configurations.Content;

public sealed class ContentVersionConfiguration : IEntityTypeConfiguration<ContentVersionRow>
{
    public void Configure(EntityTypeBuilder<ContentVersionRow> builder)
    {
        builder.ToTable("content_version");
        builder.HasKey(e => e.Id);
        builder.ConfigureTenantKey();
        
        builder.Property(e => e.ContentItemId).HasColumnName("content_item_id").IsRequired();
        builder.Property(e => e.VersionNumber).HasColumnName("version_number").IsRequired();
        builder.Property(e => e.Lifecycle).HasColumnName("lifecycle").HasMaxLength(50).IsRequired();
        builder.Property(e => e.WorkflowStateId).HasColumnName("workflow_state_id");
        builder.Property(e => e.PublishedAt).HasColumnName("published_at");

        builder.ConfigureAuditFields();

        builder.HasOne(e => e.ContentItem)
            .WithMany()
            .HasForeignKey(e => e.ContentItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => new { e.TenantId, e.ContentItemId, e.VersionNumber })
            .IsUnique()
            .HasDatabaseName("uq_content_version_tenant_item_number");
        builder.HasIndex(e => new { e.TenantId, e.ContentItemId, e.Lifecycle }).HasDatabaseName("ix_content_version_lifecycle");
    }
}
