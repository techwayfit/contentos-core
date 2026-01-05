using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.DomainEntities;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Configurations.DomainEntities;

public sealed class EntityDefinitionConfiguration : IEntityTypeConfiguration<EntityDefinitionRow>
{
    public void Configure(EntityTypeBuilder<EntityDefinitionRow> builder)
    {
        builder.ToTable("entity_definition");
        builder.HasKey(e => e.Id);
        builder.ConfigureTenantKey();
        
        builder.Property(e => e.ModuleId).HasColumnName("module_id").IsRequired();
        builder.Property(e => e.EntityKey).HasColumnName("entity_key").HasMaxLength(200).IsRequired();
        builder.Property(e => e.DisplayName).HasColumnName("display_name").HasMaxLength(200).IsRequired();
        builder.Property(e => e.SchemaJson).HasColumnName("schema_json").HasColumnType("jsonb").IsRequired();
        builder.Property(e => e.SupportsVersioning).HasColumnName("supports_versioning").IsRequired();
        builder.Property(e => e.SupportsWorkflow).HasColumnName("supports_workflow").IsRequired();

        builder.ConfigureAuditFields();

        builder.HasOne(e => e.Module)
            .WithMany()
            .HasForeignKey(e => e.ModuleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => new { e.TenantId, e.EntityKey })
            .IsUnique()
            .HasFilter("deleted_on IS NULL")
            .HasDatabaseName("uq_entity_definition_tenant_key");
    }
}
