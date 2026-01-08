using TechWayFit.ContentOS.Kernel;
using TechWayFit.ContentOS.Tenancy.Domain.Core;
using TechWayFit.ContentOS.Tenancy.Ports.Core;

namespace TechWayFit.ContentOS.Tenancy.Application.Sites;

/// <summary>
/// Use case: Get a site by hostname
/// </summary>
public sealed class GetSiteByHostNameUseCase
{
    private readonly ISiteRepository _repository;

    public GetSiteByHostNameUseCase(ISiteRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<Site, string>> ExecuteAsync(
        Guid tenantId,
        string hostName,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(hostName))
        {
            return Result.Fail<Site, string>("Hostname cannot be empty");
        }

        var site = await _repository.GetByHostNameAsync(tenantId, hostName.ToLowerInvariant(), cancellationToken);
        
        if (site == null)
        {
            return Result.Fail<Site, string>($"Site with hostname '{hostName}' not found");
        }

        return Result.Ok<Site, string>(site);
    }
}
