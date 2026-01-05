using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.DomainEntities;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Configurations.DomainEntities;

public sealed class EntityInstanceConfiguration : IEntityTypeConfiguration<EntityInstanceRow>
{
    public void Configure(EntityTypeBuilder<EntityInstanceRow> builder)
    {
        builder.ToTable("entity_instance");
        builder.HasKey(e => e.Id);
        builder.ConfigureTenantSiteKeys();
        
        builder.Property(e => e.EntityDefinitionId).HasColumnName("entity_definition_id").IsRequired();
        builder.Property(e => e.DataJson).HasColumnName("data_json").HasColumnType("jsonb").IsRequired();

        builder.ConfigureAuditFields();

        builder.HasOne(e => e.EntityDefinition)
            .WithMany()
            .HasForeignKey(e => e.EntityDefinitionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => new { e.TenantId, e.SiteId, e.EntityDefinitionId }).HasDatabaseName("ix_entity_instance_tenant_site_definition");
    }
}
