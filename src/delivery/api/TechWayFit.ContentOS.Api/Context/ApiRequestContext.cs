using TechWayFit.ContentOS.Infrastructure.Runtime;
using TechWayFit.ContentOS.Api.Security;
using TechWayFit.ContentOS.Api.Tenancy;

namespace TechWayFit.ContentOS.Api.Context;

/// <summary>
/// Represents the context for an API request using AsyncLocal storage.
/// Maintains context across async flows within a single request lifecycle.
/// This is runtime/transport layer concern - belongs in API layer.
/// </summary>
public class ApiRequestContext
{
    private static readonly AsyncLocal<ApiRequestContext?> _current = new();

    /// <summary>
    /// Gets the current request context from AsyncLocal storage
    /// </summary>
    public static ApiRequestContext? Current => _current.Value;

    private readonly Dictionary<string, object> _metadata = new();

    public string CorrelationId { get; private set; } = string.Empty;
    public DateTimeOffset RequestTimestamp { get; private set; }
    public string HttpMethod { get; private set; } = string.Empty;
    public string RequestPath { get; private set; } = string.Empty;
    public string? ClientIpAddress { get; private set; }
    public string? UserAgent { get; private set; }
    public CurrentUser? CurrentUser { get; private set; }
    public TenantContext? TenantContext { get; private set; }
    public LanguageContext? LanguageContext { get; private set; }
    public string? ResourceType { get; private set; }
    public string? ResourceId { get; private set; }
    public string? RequiredPermission { get; private set; }
    public bool IsSuperAdmin { get; private set; }
    public bool IsAuthenticated => CurrentUser?.IsAuthenticated ?? false;
    public IReadOnlyDictionary<string, object> Metadata => _metadata;

    /// <summary>
    /// Initializes the request context
    /// </summary>
    public void Initialize(
        string correlationId,
        string httpMethod,
        string requestPath,
        string? clientIpAddress = null,
        string? userAgent = null)
    {
        CorrelationId = correlationId;
        RequestTimestamp = DateTimeOffset.UtcNow;
        HttpMethod = httpMethod;
        RequestPath = requestPath;
        ClientIpAddress = clientIpAddress;
        UserAgent = userAgent;
    }

    /// <summary>
    /// Sets the current user context
    /// </summary>
    public void SetCurrentUser(CurrentUser? currentUser)
    {
        CurrentUser = currentUser;
    }

    /// <summary>
    /// Sets the tenant context
    /// </summary>
    public void SetTenantContext(TenantContext? tenantContext)
    {
        TenantContext = tenantContext;
    }

    /// <summary>
    /// Sets the language context
    /// </summary>
    public void SetLanguageContext(LanguageContext? languageContext)
    {
        LanguageContext = languageContext;
    }

    /// <summary>
    /// Sets the resource information
    /// </summary>
    public void SetResource(string? resourceType, string? resourceId = null)
    {
        ResourceType = resourceType;
        ResourceId = resourceId;
    }

    /// <summary>
    /// Sets the required permission for the operation
    /// </summary>
    public void SetRequiredPermission(string? permission)
    {
        RequiredPermission = permission;
    }

    /// <summary>
    /// Sets the SuperAdmin flag
    /// </summary>
    public void SetSuperAdmin(bool isSuperAdmin)
    {
        IsSuperAdmin = isSuperAdmin;
    }

    /// <summary>
    /// Adds or updates a metadata value
    /// </summary>
    public void AddMetadata(string key, object value)
    {
        _metadata[key] = value;
    }

    /// <summary>
    /// Sets the current context in AsyncLocal storage
    /// </summary>
    public void Activate()
    {
        _current.Value = this;
    }

    /// <summary>
    /// Clears the current context from AsyncLocal storage
    /// </summary>
    public static void Clear()
    {
        _current.Value = null;
    }

    /// <summary>
    /// Creates a new request context and activates it
    /// </summary>
    public static ApiRequestContext Create(
        string correlationId,
        string httpMethod,
        string requestPath,
        string? clientIpAddress = null,
        string? userAgent = null)
    {
        var context = new ApiRequestContext();
        context.Initialize(correlationId, httpMethod, requestPath, clientIpAddress, userAgent);
        context.Activate();
        return context;
    }
}
