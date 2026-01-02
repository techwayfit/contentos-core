using TechWayFit.ContentOS.Abstractions;
using TechWayFit.ContentOS.Content.Domain;
using TechWayFit.ContentOS.Content.Ports;
using TechWayFit.ContentOS.Contracts.Dtos;
using TechWayFit.ContentOS.Contracts.Events;
using TechWayFit.ContentOS.Kernel;
using TechWayFit.ContentOS.Kernel.Primitives;

namespace TechWayFit.ContentOS.Content.Application;

/// <summary>
/// Implementation of add localization use case
/// </summary>
public class AddLocalizationUseCase : IAddLocalizationUseCase
{
    private readonly IContentRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentTenantProvider _tenantContext;
    private readonly ICurrentUserProvider _currentUser;
    private readonly IEventBus _eventBus;

    public AddLocalizationUseCase(
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
        AddLocalizationCommand command,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Load content
            var content = await _repository.GetByIdAsync(command.ContentId, cancellationToken);
            if (content == null)
                return Result.Fail<ContentResponse, Error>(Error.NotFound($"Content with ID {command.ContentId} not found"));

            // Create localization
            var localization = ContentLocalization.Create(
                content.Id,
                new LanguageCode(command.LanguageCode),
                new ContentTitle(command.Title),
                new ContentSlug(command.Slug),
                new ContentFields(command.Fields)
            );

            content.AddLocalization(localization);

            // Persist
            await _repository.UpdateAsync(content, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Publish event
            await _eventBus.PublishAsync(new ContentLocalizedEventV1
            {
                TenantId = _tenantContext.TenantId,
                SiteId = _tenantContext.SiteId,
                Environment = _tenantContext.Environment,
                UserId = _currentUser.UserId,
                ContentId = content.Id.Value,
                LanguageCode = command.LanguageCode,
                Title = command.Title
            }, cancellationToken);

            // Map to response (with new localization)
            var response = new ContentResponse
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
}
