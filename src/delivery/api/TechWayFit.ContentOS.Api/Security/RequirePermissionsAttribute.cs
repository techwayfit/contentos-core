using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TechWayFit.ContentOS.Abstractions.Security;

namespace TechWayFit.ContentOS.Api.Security;

/// <summary>
/// Action filter that enforces required permissions using IPolicyEvaluator.
/// Keeps controllers thin by removing explicit permission checks in actions.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public sealed class RequirePermissionsAttribute : Attribute, IAsyncActionFilter
{
    private readonly string[] _permissions;

    public RequirePermissionsAttribute(params string[] permissions)
    {
        _permissions = permissions;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var evaluator = context.HttpContext.RequestServices.GetService(typeof(IPolicyEvaluator)) as IPolicyEvaluator
            ?? throw new InvalidOperationException("IPolicyEvaluator is not registered in the service container.");

        var cancellationToken = context.HttpContext.RequestAborted;

        foreach (var permission in _permissions)
        {
            await evaluator.RequireAsync(permission, cancellationToken);
        }

        await next();
    }
}
