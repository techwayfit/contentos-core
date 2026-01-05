using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Content;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Configurations.Content;

public sealed class ContentItemConfiguration : IEntityTypeConfiguration<ContentItemRow>
{
    public void Configure(EntityTypeBuilder<ContentItemRow> builder)
    {
        builder.ToTable("content_item");
        builder.HasKey(e => e.Id);
        builder.ConfigureTenantSiteKeys();
        
        builder.Property(e => e.ContentTypeId).HasColumnName("content_type_id").IsRequired();
        builder.Property(e => e.Status).HasColumnName("status").HasMaxLength(50).IsRequired();

        builder.ConfigureAuditFields();

        builder.HasOne(e => e.ContentType)
            .WithMany()
            .HasForeignKey(e => e.ContentTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => new { e.TenantId, e.SiteId, e.Status }).HasDatabaseName("ix_content_item_tenant_site_status");
        builder.HasIndex(e => new { e.TenantId, e.ContentTypeId }).HasDatabaseName("ix_content_item_tenant_type");
    }
}
