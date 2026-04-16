using TechWayFit.ContentOS.Abstractions;
using TechWayFit.ContentOS.Api.Configuration;
using TechWayFit.ContentOS.Api.Middleware;
using TechWayFit.ContentOS.Api.Security;
using TechWayFit.ContentOS.Api.Tenancy;
using TechWayFit.ContentOS.Content;
using TechWayFit.ContentOS.Infrastructure.Events;
using TechWayFit.ContentOS.Infrastructure.Persistence.Postgres;
using TechWayFit.ContentOS.Infrastructure.Runtime;
using TechWayFit.ContentOS.Kernel;
using TechWayFit.ContentOS.Abstractions.Security;
using TechWayFit.ContentOS.Kernel.Security;
using TechWayFit.ContentOS.Media;
using TechWayFit.ContentOS.Search;
using TechWayFit.ContentOS.Tenancy;
using TechWayFit.ContentOS.Workflow.Application;
using TechWayFit.ContentOS.Infrastructure.Identity;

var builder = WebApplication.CreateBuilder(args);

// ============================================================================
// CORE SERVICES
// ============================================================================

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ============================================================================
// SECURITY CONFIGURATION
// ============================================================================

// Add authentication and authorization (Phase 1 implementation)
builder.Services.AddIdentity(builder.Configuration);

builder.Services.AddAuthorization(options =>
{
    // SuperAdmin policy (Platform-level access)
    options.AddPolicy("SuperAdmin", policy =>
  policy.RequireAssertion(context =>
      {
 // MVP: Header-based, Future: JWT claim
    var httpContext = context.Resource as Microsoft.AspNetCore.Http.DefaultHttpContext;
       var headerValue = httpContext?.Request.Headers["X-SuperAdmin"].FirstOrDefault();
     return string.Equals(headerValue, "true", StringComparison.OrdinalIgnoreCase) ||
    context.User.HasClaim("scope", "platform:superadmin");
        }));

    // Tenant admin policy
    options.AddPolicy("TenantAdmin", policy =>
     policy.RequireClaim("role", "TenantAdmin"));

    // Content management policies
    options.AddPolicy("Content.Create", policy =>
        policy.RequireClaim("permission", "content:create"));
    
    options.AddPolicy("Content.Edit", policy =>
policy.RequireClaim("permission", "content:edit"));
    
    options.AddPolicy("Content.Publish", policy =>
  policy.RequireClaim("permission", "content:publish"));
    
    options.AddPolicy("Content.Delete", policy =>
  policy.RequireClaim("permission", "content:delete"));
});

// ============================================================================
// SWAGGER/OPENAPI DOCUMENTATION
// ============================================================================

builder.Services.AddSwaggerDocumentation();

// ============================================================================
// KERNEL & RUNTIME SERVICES
// ============================================================================

// Add kernel services (platform context services)
builder.Services.AddKernelServices();

// Add runtime context services
builder.Services.AddScoped<TenantContext>();
builder.Services.AddScoped<CurrentUser>();
builder.Services.AddScoped<LanguageContext>();
builder.Services.AddScoped<ITenantResolver, TenantResolver>();

// Register interface adapters so feature projects only depend on Abstractions
builder.Services.AddScoped<ICurrentTenantProvider>(sp => sp.GetRequiredService<TenantContext>());
builder.Services.AddScoped<ICurrentUserProvider>(sp => sp.GetRequiredService<CurrentUser>());
builder.Services.AddScoped<ICurrentLocaleProvider>(sp => sp.GetRequiredService<LanguageContext>());

// Add event bus (infrastructure implementation)
builder.Services.AddInMemoryEventBus();

// Add API request context
builder.Services.AddApiRequestContext();

// Add SuperAdmin context (MVP: header-based, later: JWT)
builder.Services.AddScoped<ISuperAdminContext>(sp =>
{
    var httpContext = sp.GetRequiredService<IHttpContextAccessor>().HttpContext
 ?? throw new InvalidOperationException("No HttpContext available");

    var headerValue = httpContext.Request.Headers["X-SuperAdmin"].ToString();
    var isSuperAdmin = string.Equals(headerValue, "true", StringComparison.OrdinalIgnoreCase);

    return new HeaderSuperAdminContext(isSuperAdmin);
});

// Add policy evaluator
builder.Services.AddScoped<IPolicyEvaluator, MvpPolicyEvaluator>();

// ============================================================================
// INFRASTRUCTURE SERVICES
// ============================================================================

// Add PostgreSQL persistence
builder.Services.AddPostgresPersistence(builder.Configuration);

// ============================================================================
// FEATURE SERVICES
// ============================================================================

// Add Tenancy feature
builder.Services.AddTenancy();

// Add Content feature
builder.Services.AddContent();

// Add workflow use case (stub)
builder.Services.AddScoped<ITransitionWorkflowUseCase, TransitionWorkflowUseCase>();

// Add media services
builder.Services.AddMediaServices();

// Add search services
builder.Services.AddSearchServices();

// ============================================================================
// BUILD APPLICATION
// ============================================================================

var app = builder.Build();

// ============================================================================
// MIDDLEWARE PIPELINE (ORDER MATTERS!)
// ============================================================================

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerDocumentation();
}

app.UseHttpsRedirection();

// 1. API request context (must come first)
app.UseApiRequestContext();

// 2. Tenant resolution (validates X-Tenant-Id header)
app.UseMiddleware<TenantResolutionMiddleware>();

// 3. Legacy tenant middleware (populates TenantContext)
app.UseMiddleware<TenantMiddleware>();

// 4. Authentication (validates JWT/credentials)
app.UseAuthentication();

// 5. Authorization (enforces policies)
app.UseAuthorization();

// ============================================================================
// ENDPOINT MAPPING
// ============================================================================

app.MapControllers();

app.MapGet("/", () => new 
{
    Name = "TechWayFit ContentOS Core API",
    Version = "1.0.0",
    Description = "Headless, API-first content management platform",
    Documentation = "/swagger",
    Security = new
    {
        Authentication = "JWT Bearer (MVP: Development mode)",
        Authorization = "Policy-based with custom requirements",
        MultiTenancy = "Header-based (X-Tenant-Id required for tenant routes)"
  },
    Features = new[]
    {
        "Multi-tenant architecture",
        "Multi-language content support",
        "Event-driven design",
      "Clean architecture with DDD",
        "PostgreSQL with EF Core",
        "JWT Authentication",
        "Policy-based Authorization"
    }
})
.WithName("GetApiInfo")
.WithTags("System")
.Produces(200);

app.Run();

// Make the implicit Program class public so test projects can access it
public partial class Program { }
