using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Core;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Configurations.Core;

/// <summary>
/// EF Core configuration for RoleRow - RBAC roles
/// </summary>
public sealed class RoleConfiguration : IEntityTypeConfiguration<RoleRow>
{
    public void Configure(EntityTypeBuilder<RoleRow> builder)
    {
        builder.ToTable("role");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");
        
        builder.Property(e => e.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(e => e.Name).HasColumnName("name").HasMaxLength(100).IsRequired();

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
        builder.HasIndex(e => e.TenantId).HasDatabaseName("ix_role_tenant_id");
        builder.HasIndex(e => new { e.TenantId, e.Name })
            .IsUnique()
            .HasFilter("deleted_on IS NULL")
            .HasDatabaseName("uq_role_tenant_name");
    }
}
