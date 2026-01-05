using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Core;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Configurations.Core;

/// <summary>
/// EF Core configuration for SiteRow - Multi-site within a tenant
/// </summary>
public sealed class SiteConfiguration : IEntityTypeConfiguration<SiteRow>
{
    public void Configure(EntityTypeBuilder<SiteRow> builder)
    {
        builder.ToTable("site");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");
        
        builder.Property(e => e.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(e => e.Name).HasColumnName("name").HasMaxLength(200).IsRequired();
        builder.Property(e => e.HostName).HasColumnName("host_name").HasMaxLength(255).IsRequired();
        builder.Property(e => e.DefaultLocale).HasColumnName("default_locale").HasMaxLength(10).IsRequired();

        // Audit fields
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

        // Foreign keys
        builder.HasOne(e => e.Tenant)
            .WithMany()
            .HasForeignKey(e => e.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(e => e.TenantId).HasDatabaseName("ix_site_tenant_id");
        builder.HasIndex(e => new { e.TenantId, e.HostName })
            .IsUnique()
            .HasFilter("deleted_on IS NULL")
            .HasDatabaseName("uq_site_tenant_hostname");
    }
}
