using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechWayFit.ContentOS.Infrastructure.Persistence.Conventions;
using TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Entities;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Configurations;

/// <summary>
/// EF Core configuration for TenantRow
/// </summary>
public sealed class TenantConfiguration : IEntityTypeConfiguration<TenantRow>
{
    public void Configure(EntityTypeBuilder<TenantRow> builder)
    {
        builder.ToTable(NamingConventions.ToTableName(nameof(TenantRow)));

        builder.HasKey(e => e.Id);

        // Unique key (slug) - MUST be globally unique
        builder.Property(e => e.Key)
            .HasColumnName("key")
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(e => e.Key)
            .IsUnique()
            .HasDatabaseName("uq_tenants_key");

        // Tenant name
        builder.Property(e => e.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        // Status (Active=0, Disabled=1)
        builder.Property(e => e.Status)
            .HasColumnName("status")
            .IsRequired();

        builder.HasIndex(e => e.Status)
            .HasDatabaseName("ix_tenants_status");

        // Audit fields
        builder.Property(e => e.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .HasColumnName("updated_at");
    }
}
