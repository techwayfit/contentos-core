using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Modules;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Configurations.Modules;

public sealed class ModuleSettingConfiguration : IEntityTypeConfiguration<ModuleSettingRow>
{
    public void Configure(EntityTypeBuilder<ModuleSettingRow> builder)
    {
        builder.ToTable("module_setting");
        builder.HasKey(e => e.Id);
        builder.ConfigureTenantKey();
        
        builder.Property(e => e.ModuleId).HasColumnName("module_id").IsRequired();
        builder.Property(e => e.SettingKey).HasColumnName("setting_key").HasMaxLength(200).IsRequired();
        builder.Property(e => e.ValueJson).HasColumnName("value_json").HasColumnType("jsonb").IsRequired();

        builder.ConfigureAuditFields();

        builder.HasOne(e => e.Module)
            .WithMany()
            .HasForeignKey(e => e.ModuleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => new { e.TenantId, e.ModuleId, e.SettingKey })
            .IsUnique()
            .HasFilter("deleted_on IS NULL")
            .HasDatabaseName("uq_module_setting_tenant_module_key");
    }
}
