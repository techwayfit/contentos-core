using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Entities;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Configurations;

/// <summary>
/// EF Core configuration for ContentLocalizationRow
/// </summary>
public class ContentLocalizationConfiguration : IEntityTypeConfiguration<ContentLocalizationRow>
{
    public void Configure(EntityTypeBuilder<ContentLocalizationRow> builder)
    {
        builder.ToTable("content_localizations");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.ContentItemId)
            .IsRequired()
            .HasColumnName("content_item_id");

        builder.Property(x => x.LanguageCode)
            .IsRequired()
            .HasMaxLength(10)
            .HasColumnName("language_code");

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(500)
            .HasColumnName("title");

        builder.Property(x => x.Slug)
            .IsRequired()
            .HasMaxLength(500)
            .HasColumnName("slug");

        builder.Property(x => x.FieldsJson)
            .IsRequired()
            .HasColumnType("jsonb") // PostgreSQL JSONB type
            .HasDefaultValue("{}")
            .HasColumnName("fields_json");

        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");

        builder.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at");

        // Unique constraint: one localization per language per content item
        builder.HasIndex(x => new { x.ContentItemId, x.LanguageCode })
            .IsUnique()
            .HasDatabaseName("ix_content_localizations_unique");

        // Index for language queries
        builder.HasIndex(x => x.LanguageCode)
            .HasDatabaseName("ix_content_localizations_language");

        // Index for slug lookups
        builder.HasIndex(x => x.Slug)
            .HasDatabaseName("ix_content_localizations_slug");

        // GIN index for JSONB fields (PostgreSQL full-text search support)
        // Note: This uses raw SQL as EF Core doesn't have built-in JSONB GIN support
        // The actual index will be created in migrations
    }
}
