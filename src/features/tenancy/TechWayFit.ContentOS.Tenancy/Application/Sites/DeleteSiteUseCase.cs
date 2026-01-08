using TechWayFit.ContentOS.Abstractions;
using TechWayFit.ContentOS.Kernel;
using TechWayFit.ContentOS.Tenancy.Ports.Core;

namespace TechWayFit.ContentOS.Tenancy.Application.Sites;

/// <summary>
/// Use case: Delete a site from tenant
/// </summary>
public sealed class DeleteSiteUseCase
{
    private readonly ISiteRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteSiteUseCase(ISiteRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool, string>> ExecuteAsync(
        Guid siteId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        // Retrieve site
        var site = await _repository.GetByIdAsync(siteId, cancellationToken);
        if (site == null || site.TenantId != tenantId)
        {
            return Result.Fail<bool, string>($"Site with ID '{siteId}' not found in this tenant");
        }

        // TODO: Check for content dependencies
        // TODO: Prevent deletion if content exists for this site
        // TODO: Check if this is the default site - prevent deletion

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            // Delete site
            await _repository.DeleteAsync(siteId, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            // TODO: Publish SiteDeleted domain event
            // TODO: Archive site data
            // TODO: Update DNS/routing configurations

            return Result.Ok<bool, string>(true);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result.Fail<bool, string>($"Failed to delete site: {ex.Message}");
        }
    }
}
