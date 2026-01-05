using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Content;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Configurations.Content;

public sealed class ContentTypeFieldConfiguration : IEntityTypeConfiguration<ContentTypeFieldRow>
{
    public void Configure(EntityTypeBuilder<ContentTypeFieldRow> builder)
    {
        builder.ToTable("content_type_field");
        builder.HasKey(e => e.Id);
        builder.ConfigureTenantKey();
        
        builder.Property(e => e.ContentTypeId).HasColumnName("content_type_id").IsRequired();
        builder.Property(e => e.FieldKey).HasColumnName("field_key").HasMaxLength(100).IsRequired();
        builder.Property(e => e.DataType).HasColumnName("data_type").HasMaxLength(50).IsRequired();
        builder.Property(e => e.IsRequired).HasColumnName("is_required").IsRequired();
        builder.Property(e => e.IsLocalized).HasColumnName("is_localized").IsRequired();
        builder.Property(e => e.ConstraintsJson).HasColumnName("constraints_json").HasColumnType("jsonb").IsRequired();
        builder.Property(e => e.SortOrder).HasColumnName("sort_order").IsRequired();

        builder.ConfigureAuditFields();

        builder.HasOne(e => e.ContentType)
            .WithMany()
            .HasForeignKey(e => e.ContentTypeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => new { e.TenantId, e.ContentTypeId, e.FieldKey })
            .IsUnique()
            .HasFilter("deleted_on IS NULL")
            .HasDatabaseName("uq_content_type_field_tenant_type_key");
    }
}
