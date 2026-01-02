using TechWayFit.ContentOS.Kernel.Security;

namespace TechWayFit.ContentOS.Kernel.Security;

/// <summary>
/// MVP policy evaluator with SuperAdmin scope enforcement
/// </summary>
public sealed class MvpPolicyEvaluator : IPolicyEvaluator
{
    private readonly ISuperAdminContext _superAdminContext;

    public MvpPolicyEvaluator(ISuperAdminContext superAdminContext)
    {
        _superAdminContext = superAdminContext;
    }

    public Task RequireAsync(string permission, CancellationToken cancellationToken = default)
    {
        // SuperAdmin permissions require SuperAdmin context
        if (permission == AdminPermissions.SuperAdmin || permission == AdminPermissions.TenantManage)
        {
            if (!_superAdminContext.IsSuperAdmin)
            {
                throw new UnauthorizedAccessException($"SuperAdmin scope required for permission: {permission}");
            }
        }

        // For now, all other permissions are allowed (extend later with RBAC)
        return Task.CompletedTask;
    }
}
