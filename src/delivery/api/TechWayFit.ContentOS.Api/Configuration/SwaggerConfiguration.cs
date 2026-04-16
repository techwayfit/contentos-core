using System.Reflection;

namespace TechWayFit.ContentOS.Api.Configuration;

/// <summary>
/// Extension methods for configuring Swagger/OpenAPI documentation
/// </summary>
public static class SwaggerConfiguration
{
    /// <summary>
    /// Adds Swagger/OpenAPI documentation services
    /// </summary>
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
services.AddEndpointsApiExplorer();
        
     services.AddSwaggerGen(options =>
      {
  // API Information
            options.SwaggerDoc("v1", new()
         {
      Title = "TechWayFit ContentOS Core API",
  Version = "v1.0",
    Description = GetApiDescription(),
                Contact = new()
 {
        Name = "TechWayFit",
        Email = "support@techwayfit.com",
        Url = new Uri("https://techwayfit.com")
    },
                License = new()
                {
          Name = "Proprietary",
       Url = new Uri("https://techwayfit.com/license")
     }
      });

    // Add XML comments for better documentation
          AddXmlComments(options);

  // Enable annotations for rich metadata
            options.EnableAnnotations();

  // Schema customization
        options.CustomSchemaIds(type => type.FullName?.Replace("+", "."));
      });

return services;
    }

    /// <summary>
    /// Configures Swagger UI middleware
    /// </summary>
 public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app)
    {
        app.UseSwagger(c =>
        {
            c.RouteTemplate = "api/swagger/{documentName}/swagger.json";
 });

        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/api/swagger/v1/swagger.json", "ContentOS API v1");
            c.RoutePrefix = "api/swagger";
          c.DocumentTitle = "ContentOS API Documentation";
            c.DefaultModelsExpandDepth(2);
 c.DefaultModelRendering(Swashbuckle.AspNetCore.SwaggerUI.ModelRendering.Example);
      c.DisplayRequestDuration();
            c.EnableDeepLinking();
       c.EnableFilter();
            c.ShowExtensions();

        // Custom CSS for branding
            c.InjectStylesheet("/swagger-ui/custom.css");
        });

  return app;
    }

    #region Private Helper Methods

    private static string GetApiDescription()
    {
        return @"**Headless, API-first content management platform with multi-tenancy and multi-language support**

## Features
- ?? **Multi-tenant architecture** - Complete tenant isolation with site support
- ?? **Multi-language content** - Built-in localization and translation workflows
- ?? **Modular design** - Content, Media, Workflow, Search, and AI modules
- ?? **Event-driven** - Domain events for extensibility and integration
- ??? **PostgreSQL** - Robust persistence with EF Core
- ?? **Role-based security** - Fine-grained permissions and ACLs

## Authentication
This API uses header-based authentication (MVP):
- **Tenant**: `X-Tenant-Id` header (UUID)
- **Site**: `X-Site-Id` header (UUID)  
- **Environment**: `X-Environment` header (Draft/Published)
- **SuperAdmin**: `X-SuperAdmin` header (true/false)

## Multi-Tenancy
All endpoints (except `/api/admin/tenants`) require a valid `X-Tenant-Id` header.
SuperAdmin endpoints require `X-SuperAdmin: true` header.";
    }

    private static void AddXmlComments(Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenOptions options)
    {
var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        
   if (File.Exists(xmlPath))
    {
       options.IncludeXmlComments(xmlPath);
 }
    }

    #endregion
}
