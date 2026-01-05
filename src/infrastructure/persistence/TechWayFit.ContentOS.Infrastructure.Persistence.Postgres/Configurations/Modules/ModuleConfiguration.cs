using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Modules;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Configurations.Modules;

public sealed class ModuleConfiguration : IEntityTypeConfiguration<ModuleRow>
{
    public void Configure(EntityTypeBuilder<ModuleRow> builder)
    {
        builder.ToTable("module");
        builder.HasKey(e => e.Id);
        builder.ConfigureTenantKey();
        
        builder.Property(e => e.ModuleKey).HasColumnName("module_key").HasMaxLength(200).IsRequired();
        builder.Property(e => e.DisplayName).HasColumnName("display_name").HasMaxLength(200).IsRequired();
        builder.Property(e => e.Version).HasColumnName("version").HasMaxLength(50).IsRequired();
        builder.Property(e => e.InstallationStatus).HasColumnName("installation_status").HasMaxLength(50).IsRequired();
        builder.Property(e => e.InstalledOn).HasColumnName("installed_on").IsRequired();
        builder.Property(e => e.DependenciesJson).HasColumnName("dependencies_json").HasColumnType("jsonb").IsRequired();

        builder.ConfigureAuditFields();

        builder.HasIndex(e => new { e.TenantId, e.ModuleKey })
            .IsUnique()
            .HasFilter("deleted_on IS NULL")
            .HasDatabaseName("uq_module_tenant_key");
    }
}
