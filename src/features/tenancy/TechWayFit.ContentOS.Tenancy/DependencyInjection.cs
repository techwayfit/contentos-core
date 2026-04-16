using Microsoft.Extensions.DependencyInjection;
using TechWayFit.ContentOS.Tenancy.Application;
using TechWayFit.ContentOS.Tenancy.Application.Tenants;
using TechWayFit.ContentOS.Tenancy.Application.Sites;
using TechWayFit.ContentOS.Tenancy.Application.Users;
using TechWayFit.ContentOS.Tenancy.Application.Roles;
using TechWayFit.ContentOS.Tenancy.Application.Groups;

namespace TechWayFit.ContentOS.Tenancy;

public static class DependencyInjection
{
    public static IServiceCollection AddTenancy(this IServiceCollection services)
    {
        // Register Tenant use cases
        services.AddScoped<CreateTenantUseCase>();
        services.AddScoped<UpdateTenantUseCase>();
        services.AddScoped<GetTenantUseCase>();
        services.AddScoped<ListTenantsUseCase>();
        services.AddScoped<SuspendTenantUseCase>();
        services.AddScoped<ActivateTenantUseCase>();
        services.AddScoped<DeleteTenantUseCase>();

        // Register Site use cases
        services.AddScoped<CreateSiteUseCase>();
        services.AddScoped<UpdateSiteUseCase>();
        services.AddScoped<GetSiteUseCase>();
        services.AddScoped<GetSiteByHostNameUseCase>();
        services.AddScoped<ListSitesUseCase>();
        services.AddScoped<DeleteSiteUseCase>();

        // Register User use cases
        services.AddScoped<CreateUserUseCase>();
        services.AddScoped<UpdateUserUseCase>();
        services.AddScoped<GetUserUseCase>();
        services.AddScoped<GetUserByEmailUseCase>();
        services.AddScoped<ListUsersUseCase>();
        services.AddScoped<DeactivateUserUseCase>();
        services.AddScoped<ReactivateUserUseCase>();
        services.AddScoped<DeleteUserUseCase>();

        // Register Role use cases
        services.AddScoped<CreateRoleUseCase>();
        services.AddScoped<UpdateRoleUseCase>();
        services.AddScoped<GetRoleUseCase>();
        services.AddScoped<ListRolesUseCase>();
        services.AddScoped<DeleteRoleUseCase>();
        services.AddScoped<AssignRoleToUserUseCase>();
        services.AddScoped<RemoveRoleFromUserUseCase>();

        // Register Group use cases
        services.AddScoped<CreateGroupUseCase>();
        services.AddScoped<UpdateGroupUseCase>();
        services.AddScoped<GetGroupUseCase>();
        services.AddScoped<ListGroupsUseCase>();
        services.AddScoped<DeleteGroupUseCase>();
        services.AddScoped<AddUserToGroupUseCase>();
        services.AddScoped<RemoveUserFromGroupUseCase>();

        return services;
    }
}
