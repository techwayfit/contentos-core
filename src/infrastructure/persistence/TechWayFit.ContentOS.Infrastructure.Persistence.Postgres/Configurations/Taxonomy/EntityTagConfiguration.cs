using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Taxonomy;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Configurations.Taxonomy;

public sealed class EntityTagConfiguration : IEntityTypeConfiguration<EntityTagRow>
{
    public void Configure(EntityTypeBuilder<EntityTagRow> builder)
    {
        builder.ToTable("entity_tag");
        builder.HasKey(e => e.Id);
        builder.ConfigureTenantKey();
        
        builder.Property(e => e.EntityType).HasColumnName("entity_type").HasMaxLength(100).IsRequired();
        builder.Property(e => e.EntityId).HasColumnName("entity_id").IsRequired();
        builder.Property(e => e.TagId).HasColumnName("tag_id").IsRequired();

        builder.ConfigureAuditFields();

        builder.HasOne(e => e.Tag)
            .WithMany()
            .HasForeignKey(e => e.TagId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => new { e.TenantId, e.EntityType, e.EntityId, e.TagId })
            .IsUnique()
            .HasFilter("deleted_on IS NULL")
            .HasDatabaseName("uq_entity_tag_tenant_entity_tag");
        builder.HasIndex(e => new { e.TenantId, e.TagId }).HasDatabaseName("ix_entity_tag_tag");
    }
}
