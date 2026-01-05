using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Content;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Configurations.Content;

public sealed class ContentNodeConfiguration : IEntityTypeConfiguration<ContentNodeRow>
{
    public void Configure(EntityTypeBuilder<ContentNodeRow> builder)
    {
        builder.ToTable("content_node");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(e => e.SiteId).HasColumnName("site_id").IsRequired();
        builder.Property(e => e.ParentId).HasColumnName("parent_id");
        builder.Property(e => e.NodeType).HasColumnName("node_type").HasMaxLength(50).IsRequired();
        builder.Property(e => e.ContentItemId).HasColumnName("content_item_id");
        builder.Property(e => e.Title).HasColumnName("title").HasMaxLength(500).IsRequired();
        builder.Property(e => e.Path).HasColumnName("path").HasMaxLength(2000).IsRequired();
        builder.Property(e => e.SortOrder).HasColumnName("sort_order").IsRequired();
        builder.Property(e => e.InheritAcl).HasColumnName("inherit_acl").IsRequired();

        ConfigureAuditFields(builder);

        // Self-referencing FK
        builder.HasOne(e => e.Parent)
            .WithMany()
            .HasForeignKey(e => e.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.ContentItem)
            .WithMany()
            .HasForeignKey(e => e.ContentItemId)
            .OnDelete(DeleteBehavior.SetNull);

        // Indexes
        builder.HasIndex(e => new { e.TenantId, e.SiteId, e.Path })
            .IsUnique()
            .HasFilter("deleted_on IS NULL")
            .HasDatabaseName("uq_content_node_tenant_site_path");
        builder.HasIndex(e => new { e.TenantId, e.SiteId, e.ParentId }).HasDatabaseName("ix_content_node_parent");
    }

    private static void ConfigureAuditFields(EntityTypeBuilder<ContentNodeRow> builder)
    {
        builder.Property(e => e.CreatedOn).HasColumnName("created_on").IsRequired();
        builder.Property(e => e.CreatedBy).HasColumnName("created_by").IsRequired();
        builder.Property(e => e.UpdatedOn).HasColumnName("updated_on").IsRequired();
        builder.Property(e => e.UpdatedBy).HasColumnName("updated_by").IsRequired();
        builder.Property(e => e.DeletedOn).HasColumnName("deleted_on");
        builder.Property(e => e.DeletedBy).HasColumnName("deleted_by");
        builder.Property(e => e.IsDeleted).HasColumnName("is_deleted").IsRequired();
        builder.Property(e => e.IsActive).HasColumnName("is_active").IsRequired();
        builder.Property(e => e.CanDelete).HasColumnName("can_delete").IsRequired();
        builder.Property(e => e.IsSystem).HasColumnName("is_system").IsRequired();
    }
}
