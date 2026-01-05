using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Core;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Configurations.Core;

/// <summary>
/// EF Core configuration for UserGroupRow - Many-to-many user-group mapping
/// </summary>
public sealed class UserGroupConfiguration : IEntityTypeConfiguration<UserGroupRow>
{
    public void Configure(EntityTypeBuilder<UserGroupRow> builder)
    {
        builder.ToTable("user_group");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");
        
        builder.Property(e => e.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(e => e.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(e => e.GroupId).HasColumnName("group_id").IsRequired();

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

        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Group)
            .WithMany()
            .HasForeignKey(e => e.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(e => new { e.TenantId, e.UserId, e.GroupId })
            .IsUnique()
            .HasFilter("deleted_on IS NULL")
            .HasDatabaseName("uq_user_group_tenant_user_group");
        builder.HasIndex(e => new { e.TenantId, e.UserId }).HasDatabaseName("ix_user_group_tenant_user");
    }
}
