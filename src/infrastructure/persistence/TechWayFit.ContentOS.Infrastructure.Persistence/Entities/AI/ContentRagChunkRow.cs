namespace TechWayFit.ContentOS.Infrastructure.Persistence.Entities.AI;

/// <summary>
/// Text chunking for Retrieval Augmented Generation (RAG).
/// Manages document chunking independently of embeddings.
/// </summary>
public class ContentRagChunkRow : BaseTenantSiteEntity
{
    /// <summary>
    /// Entity type: 'content_item', 'attachment', 'comment', 'entity_instance'
    /// </summary>
    public string SourceType { get; set; } = string.Empty;

    /// <summary>
    /// FK to source entity (polymorphic reference)
    /// </summary>
    public Guid SourceId { get; set; }

    /// <summary>
    /// For versioned content (links to CONTENT_VERSION.id)
    /// </summary>
    public Guid? SourceVersionId { get; set; }

    /// <summary>
    /// 0-based chunk sequence within source document
    /// </summary>
    public int ChunkIndex { get; set; }

    /// <summary>
    /// For hierarchical chunking (section → paragraph)
    /// </summary>
    public Guid? ParentChunkId { get; set; }

    /// <summary>
    /// Original text snippet (for citations, highlighting, display)
    /// </summary>
    public string ChunkText { get; set; } = string.Empty;

    /// <summary>
    /// Token count (for cost tracking and context window planning)
    /// </summary>
    public int? ChunkTokens { get; set; }

    /// <summary>
    /// Metadata: {startOffset, endOffset, section, page, heading}
    /// </summary>
    public string? ChunkPosition { get; set; } // JSON

    /// <summary>
    /// Language/locale: 'en-US', 'es-ES', etc.
    /// </summary>
    public string? Locale { get; set; }

    // Navigation properties
    public ContentRagChunkRow? ParentChunk { get; set; }
    public ICollection<ContentRagChunkRow> ChildChunks { get; set; } = new List<ContentRagChunkRow>();
}
