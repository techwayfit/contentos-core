using TechWayFit.ContentOS.Abstractions;
using TechWayFit.ContentOS.Content.Domain;
using TechWayFit.ContentOS.Content.Ports;
using TechWayFit.ContentOS.Contracts.Dtos;
using TechWayFit.ContentOS.Contracts.Events;
using TechWayFit.ContentOS.Kernel;
using TechWayFit.ContentOS.Kernel.Primitives;
using TechWayFit.ContentOS.Abstractions.Security;
using TechWayFit.ContentOS.Kernel.Security;

namespace TechWayFit.ContentOS.Content.Application;

/// <summary>
/// Implementation of create content use case
/// </summary>
public class CreateContentUseCase : ICreateContentUseCase
{
    private readonly IContentRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentTenantProvider _tenantContext;
    private readonly ICurrentUserProvider _currentUser;
    private readonly IEventBus _eventBus;

    public CreateContentUseCase(
        IContentRepository repository,
        IUnitOfWork unitOfWork,
        ICurrentTenantProvider tenantContext,
        ICurrentUserProvider currentUser,
        IEventBus eventBus)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
        _currentUser = currentUser;
        _eventBus = eventBus;
    }

    public async Task<Result<ContentResponse, Error>> ExecuteAsync(
        CreateContentCommand command,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate user is authenticated
            if (!_currentUser.IsAuthenticated || !_currentUser.UserId.HasValue)
                return Result.Fail<ContentResponse, Error>(Error.Unauthorized("User must be authenticated to create content"));

            // Create domain entity
            var content = ContentItem.Create(
                new TenantId(_tenantContext.TenantId),
                new SiteId(_tenantContext.SiteId),
                new ContentType(command.ContentType),
                new LanguageCode(command.LanguageCode),
                new UserId(_currentUser.UserId.Value)
            );

            // Create initial localization
            var localization = ContentLocalization.Create(
                content.Id,
                new LanguageCode(command.LanguageCode),
                new ContentTitle(command.Title),
                new ContentSlug(command.Slug),
                new ContentFields(command.Fields)
            );

            content.AddLocalization(localization);

            // Persist
            await _repository.AddAsync(content, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Publish domain event
            await _eventBus.PublishAsync(new ContentCreatedEventV1
            {
                TenantId = _tenantContext.TenantId,
                SiteId = _tenantContext.SiteId,
                Environment = _tenantContext.Environment,
                UserId = _currentUser.UserId,
                ContentId = content.Id.Value,
                ContentType = command.ContentType,
                LanguageCode = command.LanguageCode,
                Title = command.Title
            }, cancellationToken);

            // Map to response
            var response = MapToResponse(content, localization);
            return Result.Ok<ContentResponse, Error>(response);
        }
        catch (ArgumentException ex)
        {
            return Result.Fail<ContentResponse, Error>(Error.Validation(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return Result.Fail<ContentResponse, Error>(Error.Conflict(ex.Message));
        }
    }

    private ContentResponse MapToResponse(ContentItem content, ContentLocalization localization)
    {
        return new ContentResponse
        {
            Id = content.Id.Value,
            ContentType = content.ContentType.Value,
            LanguageCode = localization.LanguageCode.Value,
            DefaultLanguage = content.DefaultLanguage.Value,
            Title = localization.Title.Value,
            Slug = localization.Slug.Value,
            WorkflowStatus = content.Status.ToString(),
            Fields = localization.Fields.Value,
            AvailableLanguages = content.GetAvailableLanguages().ToList(),
            CreatedAt = content.CreatedAt,
            UpdatedAt = content.UpdatedAt,
            CreatedBy = content.CreatedBy?.Value
        };
    }
}
