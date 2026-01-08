using TechWayFit.ContentOS.Abstractions;
using TechWayFit.ContentOS.Kernel;
using TechWayFit.ContentOS.Tenancy.Domain.Core;
using TechWayFit.ContentOS.Tenancy.Ports;
using TechWayFit.ContentOS.Tenancy.Ports.Core;

namespace TechWayFit.ContentOS.Tenancy.Application.Sites;

/// <summary>
/// Use case: Create a new site within a tenant
/// </summary>
public sealed class CreateSiteUseCase
{
    private readonly ISiteRepository _siteRepository;
    private readonly ITenantRepository _tenantRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateSiteUseCase(
        ISiteRepository siteRepository,
        ITenantRepository tenantRepository,
        IUnitOfWork unitOfWork)
    {
        _siteRepository = siteRepository;
        _tenantRepository = tenantRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid, string>> ExecuteAsync(
        Guid tenantId,
        string name,
        string hostName,
        string defaultLocale,
        CancellationToken cancellationToken = default)
    {
        // Validate inputs
        if (string.IsNullOrWhiteSpace(name))
            return Result.Fail<Guid, string>("Site name cannot be empty");

        if (string.IsNullOrWhiteSpace(hostName))
            return Result.Fail<Guid, string>("Host name cannot be empty");

        if (string.IsNullOrWhiteSpace(defaultLocale))
            return Result.Fail<Guid, string>("Default locale cannot be empty");

        // Validate tenant exists
        var tenant = await _tenantRepository.GetByIdAsync(tenantId, cancellationToken);
        if (tenant == null)
        {
            return Result.Fail<Guid, string>($"Tenant with ID '{tenantId}' not found");
        }

        // Check if hostname already exists within tenant
        if (await _siteRepository.HostNameExistsAsync(tenantId, hostName, cancellationToken))
        {
            return Result.Fail<Guid, string>($"Site with hostname '{hostName}' already exists in this tenant");
        }

        // Create site entity
        var site = new Site
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Name = name,
            HostName = hostName.ToLowerInvariant(),
            DefaultLocale = defaultLocale,
            Audit = new AuditInfo
            {
                CreatedOn = DateTime.UtcNow
            }
        };

        // Persist
        await _siteRepository.AddAsync(site, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // TODO: Publish SiteCreated domain event
        // TODO: Initialize default site configurations

        return Result.Ok<Guid, string>(site.Id);
    }
}
