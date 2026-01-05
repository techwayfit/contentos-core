using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Configurations;

/// <summary>
/// Base configuration helper for audit fields
/// </summary>
public static class AuditFieldsConfigurationExtensions
{
    public static void ConfigureAuditFields<TEntity>(this EntityTypeBuilder<TEntity> builder)
        where TEntity : BaseEntity
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

    public static void ConfigureTenantSiteKeys<TEntity>(this EntityTypeBuilder<TEntity> builder)
        where TEntity : BaseTenantSiteEntity
    {
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(e => e.SiteId).HasColumnName("site_id").IsRequired();
    }

    public static void ConfigureTenantKey<TEntity>(this EntityTypeBuilder<TEntity> builder)
        where TEntity : BaseTenantEntity
    {
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.TenantId).HasColumnName("tenant_id").IsRequired();
    }
}
