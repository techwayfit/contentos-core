using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Content;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Configurations.Content;

public sealed class ContentTypeConfiguration : IEntityTypeConfiguration<ContentTypeRow>
{
    public void Configure(EntityTypeBuilder<ContentTypeRow> builder)
    {
        builder.ToTable("content_type");
        builder.HasKey(e => e.Id);
        builder.ConfigureTenantKey();
        
        builder.Property(e => e.TypeKey).HasColumnName("type_key").HasMaxLength(200).IsRequired();
        builder.Property(e => e.DisplayName).HasColumnName("display_name").HasMaxLength(200).IsRequired();
        builder.Property(e => e.SchemaVersion).HasColumnName("schema_version").IsRequired();
        builder.Property(e => e.SettingsJson).HasColumnName("settings_json").HasColumnType("jsonb").IsRequired();

        builder.ConfigureAuditFields();

        builder.HasIndex(e => new { e.TenantId, e.TypeKey })
            .IsUnique()
            .HasFilter("deleted_on IS NULL")
            .HasDatabaseName("uq_content_type_tenant_key");
    }
}
