using TechWayFit.ContentOS.Abstractions;
using TechWayFit.ContentOS.Content.Domain;
using TechWayFit.ContentOS.Content.Ports;
using TechWayFit.ContentOS.Contracts.Dtos;
using TechWayFit.ContentOS.Contracts.Events;
using TechWayFit.ContentOS.Kernel;
using TechWayFit.ContentOS.Kernel.Primitives;
using TechWayFit.ContentOS.Abstractions.Security;
using TechWayFit.ContentOS.Kernel.Security;
using TechWayFit.ContentOS.Workflow.Domain;
using TechWayFit.ContentOS.Workflow.Ports;

namespace TechWayFit.ContentOS.Workflow.Application;

/// <summary>
/// Implementation of workflow transition use case
/// </summary>
public class TransitionWorkflowUseCase : ITransitionWorkflowUseCase
{
    private readonly IWorkflowRepository _workflowRepository;
    private readonly IContentRepository _contentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentTenantProvider _tenantContext;
    private readonly ICurrentUserProvider _currentUser;
    private readonly IEventBus _eventBus;

    public TransitionWorkflowUseCase(
        IWorkflowRepository workflowRepository,
        IContentRepository contentRepository,
        IUnitOfWork unitOfWork,
        ICurrentTenantProvider tenantContext,
        ICurrentUserProvider currentUser,
        IEventBus eventBus)
    {
        _workflowRepository = workflowRepository;
        _contentRepository = contentRepository;
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
        _currentUser = currentUser;
        _eventBus = eventBus;
    }

    public async Task<Result<WorkflowStateResponse, Error>> ExecuteAsync(
        TransitionWorkflowCommand command,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate user
            if (!_currentUser.IsAuthenticated || !_currentUser.UserId.HasValue)
                return Result.Fail<WorkflowStateResponse, Error>(Error.Unauthorized("User must be authenticated"));

            // Load content
            var content = await _contentRepository.GetByIdAsync(command.ContentId, cancellationToken);
            if (content == null)
                return Result.Fail<WorkflowStateResponse, Error>(Error.NotFound($"Content with ID {command.ContentId} not found"));

            // Load or create workflow state
            var workflowState = await _workflowRepository.GetByContentIdAsync(command.ContentId, cancellationToken);
            if (workflowState == null)
            {
                workflowState = WorkflowState.CreateInitial(command.ContentId, new UserId(_currentUser.UserId.Value));
                await _workflowRepository.AddAsync(workflowState, cancellationToken);
            }

            // Parse target status
            if (!Enum.TryParse<WorkflowStatus>(command.TargetState, true, out var targetStatus))
                return Result.Fail<WorkflowStateResponse, Error>(Error.Validation($"Invalid workflow state: {command.TargetState}"));

            // Transition
            var previousStatus = workflowState.CurrentStatus;
            workflowState.Transition(targetStatus, new UserId(_currentUser.UserId.Value), command.Comment);

            // Update content status
            content.ChangeStatus(targetStatus, new UserId(_currentUser.UserId.Value));

            // Persist
            await _workflowRepository.UpdateAsync(workflowState, cancellationToken);
            await _contentRepository.UpdateAsync(content, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Publish transition event
            await _eventBus.PublishAsync(new WorkflowTransitionedEventV1
            {
                TenantId = _tenantContext.TenantId,
                SiteId = _tenantContext.SiteId,
                Environment = _tenantContext.Environment,
                UserId = _currentUser.UserId,
                ContentId = command.ContentId.Value,
                FromState = previousStatus.ToString(),
                ToState = targetStatus.ToString(),
                Comment = command.Comment
            }, cancellationToken);

            // If published, also fire ContentPublishedEventV1
            if (targetStatus == WorkflowStatus.Published)
            {
                var defaultLocalization = content.GetLocalization(content.DefaultLanguage);
                if (defaultLocalization != null)
                {
                    await _eventBus.PublishAsync(new ContentPublishedEventV1
                    {
                        TenantId = _tenantContext.TenantId,
                        SiteId = _tenantContext.SiteId,
                        Environment = _tenantContext.Environment,
                        UserId = _currentUser.UserId,
                        ContentId = content.Id.Value,
                        ContentType = content.ContentType.Value,
                        LanguageCode = defaultLocalization.LanguageCode.Value,
                        Slug = defaultLocalization.Slug.Value
                    }, cancellationToken);
                }
            }

            // Map to response
            var response = new WorkflowStateResponse
            {
                ContentId = workflowState.ContentItemId.Value,
                CurrentStatus = workflowState.CurrentStatus.ToString(),
                PreviousStatus = workflowState.PreviousStatus?.ToString(),
                TransitionedBy = workflowState.TransitionedBy?.Value,
                TransitionedAt = workflowState.TransitionedAt,
                Comment = workflowState.Comment,
                AllowedTransitions = workflowState.GetAllowedTransitions().Select(s => s.ToString()).ToList()
            };

            return Result.Ok<WorkflowStateResponse, Error>(response);
        }
        catch (InvalidOperationException ex)
        {
            return Result.Fail<WorkflowStateResponse, Error>(Error.Validation(ex.Message));
        }
    }
}
