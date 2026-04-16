using TechWayFit.ContentOS.Abstractions;
using TechWayFit.ContentOS.Content.Domain.Core;
using TechWayFit.ContentOS.Content.Ports.Core;
using TechWayFit.ContentOS.Kernel;

namespace TechWayFit.ContentOS.Content.Application.ContentItems;

/// <summary>
/// Use case: Create a new content item
/// </summary>
public sealed class CreateContentItemUseCase
{
    private readonly IContentItemRepository _contentItemRepository;
    private readonly IContentTypeRepository _contentTypeRepository;
    private readonly IContentVersionRepository _versionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateContentItemUseCase(
        IContentItemRepository contentItemRepository,
        IContentTypeRepository contentTypeRepository,
        IContentVersionRepository versionRepository,
        IUnitOfWork unitOfWork)
    {
    _contentItemRepository = contentItemRepository;
        _contentTypeRepository = contentTypeRepository;
        _versionRepository = versionRepository;
  _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid, string>> ExecuteAsync(
        Guid tenantId,
        Guid siteId,
        Guid contentTypeId,
        CancellationToken cancellationToken = default)
    {
        // Validate content type exists
        var contentType = await _contentTypeRepository.GetByIdAsync(contentTypeId, cancellationToken);
        if (contentType == null || contentType.TenantId != tenantId)
        {
   return Result.Fail<Guid, string>($"Content type with ID '{contentTypeId}' not found");
}

  // Create content item
  var contentItem = new ContentItem
    {
     Id = Guid.NewGuid(),
    TenantId = tenantId,
            SiteId = siteId,
            ContentTypeId = contentTypeId,
         Status = "draft",
            Audit = new AuditInfo
            {
     CreatedOn = DateTime.UtcNow
            }
        };

  // Create initial draft version
        var version = new ContentVersion
   {
     Id = Guid.NewGuid(),
   TenantId = tenantId,
         ContentItemId = contentItem.Id,
            VersionNumber = 1,
   Lifecycle = "draft",
            Audit = new AuditInfo
     {
        CreatedOn = DateTime.UtcNow
    }
        };

        // Persist
        await _contentItemRepository.AddAsync(contentItem, cancellationToken);
        await _versionRepository.AddAsync(version, cancellationToken);
     await _unitOfWork.SaveChangesAsync(cancellationToken);

        // TODO: Publish ContentItemCreated event

        return Result.Ok<Guid, string>(contentItem.Id);
    }
}
