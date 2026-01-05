using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.AI;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Configurations.AI;

public class ContentEmbeddingConfiguration : IEntityTypeConfiguration<ContentEmbeddingRow>
{
    public void Configure(EntityTypeBuilder<ContentEmbeddingRow> builder)
    {
        builder.ToTable("content_embedding");

        // Primary key
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");

        // Tenant and Site scoping
        builder.ConfigureTenantSiteKeys();

        // Chunk reference
        builder.Property(e => e.ChunkId)
            .IsRequired()
            .HasColumnName("chunk_id");

        // Embedding model
        builder.Property(e => e.EmbeddingModel)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("embedding_model");

        builder.Property(e => e.EmbeddingDimension)
            .IsRequired()
            .HasColumnName("embedding_dimension");

        // Vector column - pgvector extension
        // Note: Actual vector type mapping requires pgvector extension
        // This will be mapped as text for now, can be enhanced with Npgsql.EntityFrameworkCore.PostgreSQL.Vector
        builder.Property(e => e.EmbeddingVector)
            .IsRequired()
            .HasColumnType("vector")
            .HasColumnName("embedding_vector");

        // Locale
        builder.Property(e => e.Locale)
            .HasMaxLength(10)
            .HasColumnName("locale");

        // Tags array
        builder.Property(e => e.Tags)
            .HasColumnType("text[]")
            .HasColumnName("tags");

        // Metadata
        builder.Property(e => e.Metadata)
            .HasColumnType("jsonb")
            .HasColumnName("metadata");

        // Quality and timing
        builder.Property(e => e.QualityScore)
            .HasColumnName("quality_score");

        builder.Property(e => e.IndexedAt)
            .IsRequired()
            .HasColumnName("indexed_at");

        builder.Property(e => e.ExpiresAt)
            .HasColumnName("expires_at");

        // Audit fields
        builder.ConfigureAuditFields();

        // Relationships
        builder.HasOne(e => e.Chunk)
            .WithMany()
            .HasForeignKey(e => e.ChunkId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        // Note: Vector indexes (HNSW, IVFFlat) must be created via migration SQL
        // They cannot be defined in fluent API directly
        
        builder.HasIndex(e => new { e.TenantId, e.SiteId, e.Locale, e.IsActive })
            .HasDatabaseName("idx_content_embedding_tenant_locale");

        builder.HasIndex(e => e.ChunkId)
            .HasDatabaseName("idx_content_embedding_chunk");

        builder.HasIndex(e => new { e.EmbeddingModel, e.IsActive })
            .HasDatabaseName("idx_content_embedding_model");

        builder.HasIndex(e => e.Tags)
            .HasDatabaseName("idx_content_embedding_tags")
            .HasMethod("gin"); // GIN index for array search

        // Unique constraint
        builder.HasIndex(e => new { e.ChunkId, e.EmbeddingModel })
            .IsUnique()
            .HasDatabaseName("uq_content_embedding_chunk_model");

        // TODO: Add vector index via migration:
        // CREATE INDEX idx_content_embedding_vector_hnsw 
        //   ON content_embedding USING hnsw (embedding_vector vector_cosine_ops)
        //   WITH (m = 16, ef_construction = 64);
    }
}
