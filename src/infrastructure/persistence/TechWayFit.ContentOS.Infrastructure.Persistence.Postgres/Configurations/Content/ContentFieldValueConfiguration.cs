using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Content;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Configurations.Content;

public sealed class ContentFieldValueConfiguration : IEntityTypeConfiguration<ContentFieldValueRow>
{
    public void Configure(EntityTypeBuilder<ContentFieldValueRow> builder)
    {
        builder.ToTable("content_field_value");
        builder.HasKey(e => e.Id);
        builder.ConfigureTenantKey();
        
        builder.Property(e => e.ContentVersionId).HasColumnName("content_version_id").IsRequired();
        builder.Property(e => e.FieldKey).HasColumnName("field_key").HasMaxLength(100).IsRequired();
        builder.Property(e => e.Locale).HasColumnName("locale").HasMaxLength(10);
        builder.Property(e => e.ValueJson).HasColumnName("value_json").HasColumnType("jsonb").IsRequired();

        builder.ConfigureAuditFields();

        builder.HasOne(e => e.ContentVersion)
            .WithMany()
            .HasForeignKey(e => e.ContentVersionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => new { e.TenantId, e.ContentVersionId, e.FieldKey, e.Locale })
            .IsUnique()
            .HasFilter("deleted_on IS NULL")
            .HasDatabaseName("uq_content_field_value_version_field_locale");
    }
}
