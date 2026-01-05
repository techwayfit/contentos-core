using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Layout;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Configurations.Layout;

public sealed class LayoutDefinitionConfiguration : IEntityTypeConfiguration<LayoutDefinitionRow>
{
    public void Configure(EntityTypeBuilder<LayoutDefinitionRow> builder)
    {
        builder.ToTable("layout_definition");
        builder.HasKey(e => e.Id);
        builder.ConfigureTenantKey();
        
        builder.Property(e => e.LayoutKey).HasColumnName("layout_key").HasMaxLength(200).IsRequired();
        builder.Property(e => e.DisplayName).HasColumnName("display_name").HasMaxLength(200).IsRequired();
        builder.Property(e => e.RegionsRulesJson).HasColumnName("regions_rules_json").HasColumnType("jsonb").IsRequired();
        builder.Property(e => e.Version).HasColumnName("version").IsRequired();

        builder.ConfigureAuditFields();

        builder.HasIndex(e => new { e.TenantId, e.LayoutKey, e.Version })
            .IsUnique()
            .HasFilter("deleted_on IS NULL")
            .HasDatabaseName("uq_layout_definition_tenant_key_version");
    }
}
