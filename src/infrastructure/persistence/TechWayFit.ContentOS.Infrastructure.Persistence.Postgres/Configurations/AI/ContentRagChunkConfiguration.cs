using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.AI;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Configurations.AI;

public class ContentRagChunkConfiguration : IEntityTypeConfiguration<ContentRagChunkRow>
{
    public void Configure(EntityTypeBuilder<ContentRagChunkRow> builder)
    {
        builder.ToTable("content_rag_chunks");

        // Primary key
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");

        // Tenant and Site scoping
        builder.ConfigureTenantSiteKeys();

        // Source tracking
        builder.Property(e => e.SourceType)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnName("source_type");

        builder.Property(e => e.SourceId)
            .IsRequired()
            .HasColumnName("source_id");

        builder.Property(e => e.SourceVersionId)
            .HasColumnName("source_version_id");

        // Chunking
        builder.Property(e => e.ChunkIndex)
            .IsRequired()
            .HasColumnName("chunk_index");

        builder.Property(e => e.ParentChunkId)
            .HasColumnName("parent_chunk_id");

        builder.Property(e => e.ChunkText)
            .IsRequired()
            .HasColumnName("chunk_text");

        builder.Property(e => e.ChunkTokens)
            .HasColumnName("chunk_tokens");

        builder.Property(e => e.ChunkPosition)
            .HasColumnType("jsonb")
            .HasColumnName("chunk_position");

        // Locale
        builder.Property(e => e.Locale)
            .HasMaxLength(10)
            .HasColumnName("locale");

        // Audit fields
        builder.ConfigureAuditFields();

        // Relationships
        builder.HasOne(e => e.ParentChunk)
            .WithMany(e => e.ChildChunks)
            .HasForeignKey(e => e.ParentChunkId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(e => new { e.TenantId, e.SiteId, e.SourceType, e.IsActive })
            .HasDatabaseName("idx_content_rag_chunks_tenant_site_source");

        builder.HasIndex(e => new { e.SourceType, e.SourceId })
            .HasDatabaseName("idx_content_rag_chunks_source");

        builder.HasIndex(e => new { e.TenantId, e.Locale, e.IsActive })
            .HasDatabaseName("idx_content_rag_chunks_locale");

        // Unique constraint
        builder.HasIndex(e => new { e.TenantId, e.SourceType, e.SourceId, e.SourceVersionId, e.ChunkIndex })
            .IsUnique()
            .HasDatabaseName("uq_content_rag_chunks_source_chunk")
            .HasFilter("deleted_on IS NULL");
    }
}
