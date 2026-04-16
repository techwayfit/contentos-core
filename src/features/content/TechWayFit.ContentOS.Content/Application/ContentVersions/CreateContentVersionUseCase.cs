using TechWayFit.ContentOS.Abstractions;
using TechWayFit.ContentOS.Content.Domain.Core;
using TechWayFit.ContentOS.Content.Ports.Core;
using TechWayFit.ContentOS.Kernel;

namespace TechWayFit.ContentOS.Content.Application.ContentVersions;

/// <summary>
/// Use case: Create a new version of a content item
/// </summary>
public sealed class CreateContentVersionUseCase
{
    private readonly IContentItemRepository _contentItemRepository;
    private readonly IContentVersionRepository _versionRepository;
    private readonly IContentFieldValueRepository _fieldValueRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateContentVersionUseCase(
        IContentItemRepository contentItemRepository,
        IContentVersionRepository versionRepository,
        IContentFieldValueRepository fieldValueRepository,
        IUnitOfWork unitOfWork)
    {
        _contentItemRepository = contentItemRepository;
        _versionRepository = versionRepository;
        _fieldValueRepository = fieldValueRepository;
 _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid, string>> ExecuteAsync(
        Guid tenantId,
        Guid contentItemId,
  bool copyFromLatest = true,
        CancellationToken cancellationToken = default)
 {
        // Validate content item exists
        var contentItem = await _contentItemRepository.GetByIdAsync(contentItemId, cancellationToken);
        if (contentItem == null || contentItem.TenantId != tenantId)
        {
      return Result.Fail<Guid, string>($"Content item with ID '{contentItemId}' not found");
   }

        // Get existing versions to determine version number
        var existingVersions = await _versionRepository.GetByItemAsync(tenantId, contentItemId);
        var maxVersionNumber = existingVersions.Any() ? existingVersions.Max(v => v.VersionNumber) : 0;

        // Create new version
 var newVersion = new ContentVersion
 {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
         ContentItemId = contentItemId,
            VersionNumber = maxVersionNumber + 1,
     Lifecycle = "draft",
            Audit = new AuditInfo
      {
        CreatedOn = DateTime.UtcNow
 }
        };

      await _versionRepository.AddAsync(newVersion, cancellationToken);

        // Copy field values from latest version if requested
  if (copyFromLatest)
        {
        var latestVersion = existingVersions.OrderByDescending(v => v.VersionNumber).FirstOrDefault();
            if (latestVersion != null)
{
           var fieldValues = await _fieldValueRepository.GetByVersionAsync(tenantId, latestVersion.Id);
  foreach (var fieldValue in fieldValues)
 {
   var newFieldValue = new ContentFieldValue
           {
 Id = Guid.NewGuid(),
                TenantId = tenantId,
     ContentVersionId = newVersion.Id,
   FieldKey = fieldValue.FieldKey,
     Locale = fieldValue.Locale,
           ValueJson = fieldValue.ValueJson,
        Audit = new AuditInfo
       {
         CreatedOn = DateTime.UtcNow
            }
         };
 await _fieldValueRepository.AddAsync(newFieldValue, cancellationToken);
       }
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // TODO: Publish ContentVersionCreated event

      return Result.Ok<Guid, string>(newVersion.Id);
    }
}
