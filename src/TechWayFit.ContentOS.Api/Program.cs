using TechWayFit.ContentOS.Api.Middleware;
using TechWayFit.ContentOS.Api.Security;
using TechWayFit.ContentOS.Content.Application;
using TechWayFit.ContentOS.Infrastructure.Persistence.Postgres;
using TechWayFit.ContentOS.Kernel;
using TechWayFit.ContentOS.Kernel.Security;
using TechWayFit.ContentOS.Media;
using TechWayFit.ContentOS.Search;
using TechWayFit.ContentOS.Tenancy;
using TechWayFit.ContentOS.Workflow.Application;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "TechWayFit ContentOS Core API",
        Version = "v1",
        Description = "Headless, API-first content management platform with multi-tenancy and multi-language support"
    });
});

// Add kernel services (tenant context, event bus, etc.)
builder.Services.AddKernelServices();

// Add SuperAdmin context (MVP: header-based, later: JWT)
builder.Services.AddHttpContextAccessor();
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

// Add PostgreSQL persistence
builder.Services.AddPostgresPersistence(builder.Configuration);

// Add Tenancy feature
builder.Services.AddTenancy();

// Add application use cases
builder.Services.AddScoped<ICreateContentUseCase, CreateContentUseCase>();
builder.Services.AddScoped<IAddLocalizationUseCase, AddLocalizationUseCase>();
builder.Services.AddScoped<ITransitionWorkflowUseCase, TransitionWorkflowUseCase>();

// Add media services
builder.Services.AddMediaServices();

// Add search services
builder.Services.AddSearchServices();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Use tenant middleware (must come before MapControllers)
app.UseMiddleware<TenantMiddleware>();

app.MapControllers();

app.MapGet("/", () => new 
{
    Name = "TechWayFit ContentOS Core API",
    Version = "1.0.0",
    Description = "Headless, API-first content management platform",
    Features = new[]
    {
        "Multi-tenant architecture",
        "Multi-language content support",
        "Event-driven design",
        "Clean architecture with DDD",
        "PostgreSQL with EF Core"
    }
});

app.Run();

// Make the implicit Program class public so test projects can access it
public partial class Program { }
