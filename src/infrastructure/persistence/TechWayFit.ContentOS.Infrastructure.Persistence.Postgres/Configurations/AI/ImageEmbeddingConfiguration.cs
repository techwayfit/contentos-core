using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.AI;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Configurations.AI;

public class ImageEmbeddingConfiguration : IEntityTypeConfiguration<ImageEmbeddingRow>
{
    public void Configure(EntityTypeBuilder<ImageEmbeddingRow> builder)
    {
        builder.ToTable("image_embedding");

        // Primary key
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");

        // Tenant scoping
        builder.ConfigureTenantKey();

        // Attachment reference
        builder.Property(e => e.AttachmentId)
            .IsRequired()
            .HasColumnName("attachment_id");

        // Embedding model
        builder.Property(e => e.EmbeddingModel)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("embedding_model");

        builder.Property(e => e.EmbeddingDimension)
            .IsRequired()
            .HasColumnName("embedding_dimension");

        // Vector column - pgvector extension
        builder.Property(e => e.EmbeddingVector)
            .IsRequired()
            .HasColumnType("vector")
            .HasColumnName("embedding_vector");

        // Image metadata
        builder.Property(e => e.ImageMetadata)
            .HasColumnType("jsonb")
            .HasColumnName("image_metadata");

        builder.Property(e => e.OcrText)
            .HasColumnName("ocr_text");

        builder.Property(e => e.ImageQualityScore)
            .HasColumnName("image_quality_score");

        builder.Property(e => e.EmbeddingConfidence)
            .HasColumnName("embedding_confidence");

        // Audit fields
        builder.ConfigureAuditFields();

        // Relationships
        // Note: Foreign key to ATTACHMENT table
        // Navigation property omitted to avoid circular folder dependencies
        builder.HasIndex(e => e.AttachmentId)
            .HasDatabaseName("idx_image_embedding_attachment");

        // Indexes
        builder.HasIndex(e => new { e.TenantId, e.AttachmentId, e.IsActive })
            .HasDatabaseName("idx_image_embedding_tenant_attachment");

        builder.HasIndex(e => new { e.TenantId, e.IsActive, e.ImageQualityScore })
            .HasDatabaseName("idx_image_embedding_quality")
            .IsDescending(false, false, true); // Quality score descending

        builder.HasIndex(e => e.EmbeddingModel)
            .HasDatabaseName("idx_image_embedding_model");

        // Unique constraint
        builder.HasIndex(e => new { e.AttachmentId, e.EmbeddingModel })
            .IsUnique()
            .HasDatabaseName("uq_image_embedding_attachment_model");

        // Optional: Full-text search on OCR text
        // TODO: Add via migration if OCR is used:
        // CREATE INDEX idx_image_embedding_ocr_fts 
        //   ON image_embedding USING GIN (to_tsvector('english', ocr_text));

        // TODO: Add vector index via migration:
        // CREATE INDEX idx_image_embedding_vector_hnsw 
        //   ON image_embedding USING hnsw (embedding_vector vector_cosine_ops)
        //   WITH (m = 16, ef_construction = 64);
    }
}
