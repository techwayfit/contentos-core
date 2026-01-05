using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Layout;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Configurations.Layout;

public sealed class ContentLayoutConfiguration : IEntityTypeConfiguration<ContentLayoutRow>
{
    public void Configure(EntityTypeBuilder<ContentLayoutRow> builder)
    {
        builder.ToTable("content_layout");
        builder.HasKey(e => e.Id);
        builder.ConfigureTenantKey();
        
        builder.Property(e => e.ContentVersionId).HasColumnName("content_version_id").IsRequired();
        builder.Property(e => e.LayoutDefinitionId).HasColumnName("layout_definition_id");
        builder.Property(e => e.CompositionJson).HasColumnName("composition_json").HasColumnType("jsonb").IsRequired();

        builder.ConfigureAuditFields();

        builder.HasOne(e => e.ContentVersion)
            .WithMany()
            .HasForeignKey(e => e.ContentVersionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.LayoutDefinition)
            .WithMany()
            .HasForeignKey(e => e.LayoutDefinitionId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(e => new { e.TenantId, e.ContentVersionId })
            .IsUnique()
            .HasFilter("deleted_on IS NULL")
            .HasDatabaseName("uq_content_layout_tenant_version");
    }
}
