using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.DomainEntities;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Configurations.DomainEntities;

public sealed class EntityRelationshipConfiguration : IEntityTypeConfiguration<EntityRelationshipRow>
{
    public void Configure(EntityTypeBuilder<EntityRelationshipRow> builder)
    {
        builder.ToTable("entity_relationship");
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(e => e.SourceInstanceId).HasColumnName("source_instance_id").IsRequired();
        builder.Property(e => e.TargetInstanceId).HasColumnName("target_instance_id").IsRequired();
        builder.Property(e => e.RelationshipType).HasColumnName("relationship_type").HasMaxLength(100).IsRequired();
        builder.Property(e => e.RelationshipName).HasColumnName("relationship_name").HasMaxLength(200).IsRequired();
        builder.Property(e => e.MetadataJson).HasColumnName("metadata_json").HasColumnType("jsonb").IsRequired();
        builder.Property(e => e.CreatedOn).HasColumnName("created_on").IsRequired();

        builder.HasIndex(e => new { e.TenantId, e.SourceInstanceId, e.RelationshipType }).HasDatabaseName("ix_entity_relationship_source");
        builder.HasIndex(e => new { e.TenantId, e.TargetInstanceId, e.RelationshipType }).HasDatabaseName("ix_entity_relationship_target");
    }
}
