using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Taxonomy;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Configurations.Taxonomy;

public sealed class TagConfiguration : IEntityTypeConfiguration<TagRow>
{
    public void Configure(EntityTypeBuilder<TagRow> builder)
    {
        builder.ToTable("tag");
        builder.HasKey(e => e.Id);
        builder.ConfigureTenantKey();
        
        builder.Property(e => e.TagName).HasColumnName("tag_name").HasMaxLength(200).IsRequired();
        builder.Property(e => e.Taxonomy).HasColumnName("taxonomy").HasMaxLength(100).IsRequired();

        builder.ConfigureAuditFields();

        builder.HasIndex(e => new { e.TenantId, e.Taxonomy, e.TagName })
            .IsUnique()
            .HasFilter("deleted_on IS NULL")
            .HasDatabaseName("uq_tag_tenant_taxonomy_name");
    }
}
