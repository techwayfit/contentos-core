namespace TechWayFit.ContentOS.Infrastructure.Persistence.Entities.AI;

/// <summary>
/// Vector embeddings for semantic text search and RAG retrieval.
/// Links to chunks, stores embeddings separately for model flexibility.
/// </summary>
public class ContentEmbeddingRow : BaseTenantSiteEntity
{
    /// <summary>
    /// FK to CONTENT_RAG_CHUNKS.id
    /// </summary>
    public Guid ChunkId { get; set; }

    /// <summary>
    /// Model name: 'text-embedding-3-small', 'text-embedding-ada-002', 'cohere-embed-v3'
    /// </summary>
    public string EmbeddingModel { get; set; } = string.Empty;

    /// <summary>
    /// Vector size: 512, 768, 1536, 3072, etc.
    /// </summary>
    public int EmbeddingDimension { get; set; }

    /// <summary>
    /// PostgreSQL pgvector type - stored as string, mapped by EF
    /// Format: "[0.23, -0.15, 0.87, ...]"
    /// </summary>
    public string EmbeddingVector { get; set; } = string.Empty;

    /// <summary>
    /// Language/locale for language-specific search
    /// </summary>
    public string? Locale { get; set; }

    /// <summary>
    /// Tags for metadata filtering (content type, category, etc.)
    /// </summary>
    public string[]? Tags { get; set; }

    /// <summary>
    /// Extensible metadata: {contentType, category, author, etc.}
    /// </summary>
    public string? Metadata { get; set; } // JSON

    /// <summary>
    /// Optional: embedding quality/confidence score
    /// </summary>
    public float? QualityScore { get; set; }

    /// <summary>
    /// When embedding was generated
    /// </summary>
    public DateTime IndexedAt { get; set; }

    /// <summary>
    /// Optional: for cache invalidation
    /// </summary>
    public DateTime? ExpiresAt { get; set; }

    // Navigation properties
    public ContentRagChunkRow? Chunk { get; set; }
}
