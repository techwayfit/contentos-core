using TechWayFit.ContentOS.Abstractions.Repositories;

namespace TechWayFit.ContentOS.Kernel.Ports.Security;

public interface IPreviewTokenRepository : IRepository<Domain.Security.PreviewToken, Guid>
{
    Task<Domain.Security.PreviewToken?> GetByTokenHashAsync(Guid tenantId, string tokenHash);
    Task MarkUsedAsync(Guid tokenId);
    Task<int> CleanupExpiredAsync();
}
