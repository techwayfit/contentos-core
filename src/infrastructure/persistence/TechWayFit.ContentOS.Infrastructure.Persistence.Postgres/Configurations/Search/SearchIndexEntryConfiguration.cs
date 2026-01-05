using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Search;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Configurations.Search;

public sealed class SearchIndexEntryConfiguration : IEntityTypeConfiguration<SearchIndexEntryRow>
{
    public void Configure(EntityTypeBuilder<SearchIndexEntryRow> builder)
    {
        builder.ToTable("search_index_entry");
        builder.HasKey(e => e.Id);
        builder.ConfigureTenantSiteKeys();
        
        builder.Property(e => e.EntityType).HasColumnName("entity_type").HasMaxLength(100).IsRequired();
        builder.Property(e => e.EntityId).HasColumnName("entity_id").IsRequired();
        builder.Property(e => e.Locale).HasColumnName("locale").HasMaxLength(10).IsRequired();
        builder.Property(e => e.SearchableText).HasColumnName("searchable_text").IsRequired();
        builder.Property(e => e.MetadataJson).HasColumnName("metadata_json").HasColumnType("jsonb").IsRequired();
        builder.Property(e => e.IndexedAt).HasColumnName("indexed_at").IsRequired();

        builder.ConfigureAuditFields();

        builder.HasIndex(e => new { e.TenantId, e.SiteId, e.EntityType, e.EntityId, e.Locale })
            .IsUnique()
            .HasFilter("deleted_on IS NULL")
            .HasDatabaseName("uq_search_index_entry_entity_locale");
        builder.HasIndex(e => new { e.TenantId, e.SiteId, e.SearchableText })
            .HasDatabaseName("ix_search_index_entry_searchable_text");
    }
}
