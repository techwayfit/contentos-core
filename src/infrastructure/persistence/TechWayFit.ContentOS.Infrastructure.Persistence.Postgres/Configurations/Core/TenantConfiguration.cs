using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Core;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Configurations.Core;

/// <summary>
/// EF Core configuration for TenantRow - Top-level multi-tenancy boundary
/// </summary>
public sealed class TenantConfiguration : IEntityTypeConfiguration<TenantRow>
{
    public void Configure(EntityTypeBuilder<TenantRow> builder)
    {
        builder.ToTable("tenant");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.Status)
            .HasColumnName("status")
            .HasMaxLength(50)
            .IsRequired();

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

        // Indexes
        builder.HasIndex(e => e.Status).HasDatabaseName("ix_tenant_status");
        builder.HasIndex(e => new { e.IsDeleted, e.IsActive }).HasDatabaseName("ix_tenant_deleted_active");
    }
}
