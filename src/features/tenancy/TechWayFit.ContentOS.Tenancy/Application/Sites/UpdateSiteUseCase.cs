using TechWayFit.ContentOS.Abstractions;
using TechWayFit.ContentOS.Kernel;
using TechWayFit.ContentOS.Tenancy.Ports.Core;

namespace TechWayFit.ContentOS.Tenancy.Application.Sites;

/// <summary>
/// Use case: Update site metadata and configuration
/// </summary>
public sealed class UpdateSiteUseCase
{
    private readonly ISiteRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateSiteUseCase(ISiteRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool, string>> ExecuteAsync(
        Guid siteId,
        Guid tenantId,
        string? name = null,
        string? hostName = null,
        string? defaultLocale = null,
        CancellationToken cancellationToken = default)
    {
        // Retrieve site
        var site = await _repository.GetByIdAsync(siteId, cancellationToken);
        if (site == null || site.TenantId != tenantId)
        {
            return Result.Fail<bool, string>($"Site with ID '{siteId}' not found in this tenant");
        }

        // Update fields if provided
        if (!string.IsNullOrWhiteSpace(name))
        {
            site.Name = name;
        }

        if (!string.IsNullOrWhiteSpace(hostName))
        {
            // Check hostname uniqueness
            var hostNameLower = hostName.ToLowerInvariant();
            if (site.HostName != hostNameLower)
            {
                if (await _repository.HostNameExistsAsync(tenantId, hostNameLower, cancellationToken))
                {
                    return Result.Fail<bool, string>($"Hostname '{hostName}' is already in use");
                }
                site.HostName = hostNameLower;
            }
        }

        if (!string.IsNullOrWhiteSpace(defaultLocale))
        {
            site.DefaultLocale = defaultLocale;
        }

        // Update audit info
        site.Audit.UpdatedOn = DateTime.UtcNow;

        // Persist
        await _repository.UpdateAsync(site, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // TODO: Publish SiteUpdated domain event

        return Result.Ok<bool, string>(true);
    }
}
