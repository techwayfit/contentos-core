namespace TechWayFit.ContentOS.Infrastructure.Persistence.Entities.AI;

/// <summary>
/// Vector embeddings for visual similarity search and multimodal retrieval.
/// Enables image-to-image search, text-to-image search (via CLIP), and visual content discovery.
/// </summary>
public class ImageEmbeddingRow : BaseTenantEntity
{
    /// <summary>
    /// FK to ATTACHMENT.id
    /// </summary>
    public Guid AttachmentId { get; set; }

    /// <summary>
    /// Model name: 'clip-vit-large-patch14', 'dinov2-large', 'resnet50'
    /// </summary>
    public string EmbeddingModel { get; set; } = string.Empty;

    /// <summary>
    /// Vector size (typically 512, 768, 1024 for image models)
    /// </summary>
    public int EmbeddingDimension { get; set; }

    /// <summary>
    /// PostgreSQL pgvector type - stored as string, mapped by EF
    /// Format: "[0.23, -0.15, 0.87, ...]"
    /// </summary>
    public string EmbeddingVector { get; set; } = string.Empty;

    /// <summary>
    /// Image-specific metadata: width, height, aspectRatio, dominantColors, detectedObjects
    /// </summary>
    public string? ImageMetadata { get; set; } // JSON

    /// <summary>
    /// Extracted text from image (for hybrid search)
    /// </summary>
    public string? OcrText { get; set; }

    /// <summary>
    /// Optional: blur detection, aesthetic score, etc.
    /// </summary>
    public float? ImageQualityScore { get; set; }

    /// <summary>
    /// Model confidence score
    /// </summary>
    public float? EmbeddingConfidence { get; set; }

    // Navigation properties
    // Note: Navigation to AttachmentRow would be in Collaboration folder
    // Leaving it out to avoid circular dependencies between folders
}
