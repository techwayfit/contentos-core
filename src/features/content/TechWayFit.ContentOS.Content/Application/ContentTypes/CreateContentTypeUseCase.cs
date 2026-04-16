using TechWayFit.ContentOS.Abstractions;
using TechWayFit.ContentOS.Content.Domain.Core;
using TechWayFit.ContentOS.Content.Ports.Core;
using TechWayFit.ContentOS.Kernel;

namespace TechWayFit.ContentOS.Content.Application.ContentTypes;

/// <summary>
/// Use case: Create a new content type
/// </summary>
public sealed class CreateContentTypeUseCase
{
    private readonly IContentTypeRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateContentTypeUseCase(
           IContentTypeRepository repository,
           IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid, string>> ExecuteAsync(
        Guid tenantId,
        string typeKey,
        string displayName,
        CancellationToken cancellationToken = default)
    {
        // Validate inputs
        if (string.IsNullOrWhiteSpace(typeKey))
            return Result.Fail<Guid, string>("Type key cannot be empty");

        if (string.IsNullOrWhiteSpace(displayName))
            return Result.Fail<Guid, string>("Display name cannot be empty");

        // Validate type key format: lowercase alphanumeric with hyphens/underscores
        if (!System.Text.RegularExpressions.Regex.IsMatch(typeKey, "^[a-z0-9-_]+$"))
            return Result.Fail<Guid, string>("Type key must be lowercase alphanumeric with hyphens or underscores only");

        // Check if type key already exists
        if (await _repository.TypeKeyExistsAsync(tenantId, typeKey, cancellationToken))
        {
            return Result.Fail<Guid, string>($"Content type with key '{typeKey}' already exists");
        }

        // Create content type entity
        var contentType = new ContentType
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            TypeKey = typeKey,
            DisplayName = displayName,
            SchemaVersion = 1,
            SettingsJson = "{}",
            Audit = new AuditInfo
            {
                CreatedOn = DateTime.UtcNow
            }
        };

        // Persist
        await _repository.AddAsync(contentType, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // TODO: Publish ContentTypeCreated domain event

        return Result.Ok<Guid, string>(contentType.Id);
    }
}
