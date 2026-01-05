using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Layout;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Configurations.Layout;

public sealed class ComponentDefinitionConfiguration : IEntityTypeConfiguration<ComponentDefinitionRow>
{
    public void Configure(EntityTypeBuilder<ComponentDefinitionRow> builder)
    {
        builder.ToTable("component_definition");
        builder.HasKey(e => e.Id);
        builder.ConfigureTenantKey();
        
        builder.Property(e => e.ComponentKey).HasColumnName("component_key").HasMaxLength(200).IsRequired();
        builder.Property(e => e.DisplayName).HasColumnName("display_name").HasMaxLength(200).IsRequired();
        builder.Property(e => e.PropsSchemaJson).HasColumnName("props_schema_json").HasColumnType("jsonb").IsRequired();
        builder.Property(e => e.OwnerModule).HasColumnName("owner_module").HasMaxLength(100).IsRequired();
        builder.Property(e => e.Version).HasColumnName("version").IsRequired();

        builder.ConfigureAuditFields();

        builder.HasIndex(e => new { e.TenantId, e.ComponentKey, e.Version })
            .IsUnique()
            .HasFilter("deleted_on IS NULL")
            .HasDatabaseName("uq_component_definition_tenant_key_version");
    }
}
