using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TechWayFit.ContentOS.Abstractions.Security;

namespace TechWayFit.ContentOS.Infrastructure.Identity;

public static class DependencyInjection
{
    /// <summary>
    /// Adds identity and authentication services
    /// </summary>
    public static IServiceCollection AddIdentity(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var provider = configuration["Authentication:Provider"] ?? "JWT";

        switch (provider)
        {
            case "JWT":
                services.AddJwtAuthentication(configuration);
                break;
            case "OIDC":
                services.AddOidcAuthentication(configuration);
                break;
            default:
                throw new InvalidOperationException($"Unknown authentication provider: {provider}");
        }

        // Register context providers
        services.AddHttpContextAccessor();
        services.AddScoped<IAuthenticationContext, HttpAuthenticationContext>();
        services.AddScoped<ITenantContext, HttpTenantContext>();

        return services;
    }

    private static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var issuer = configuration["Authentication:JWT:Issuer"];
        var audience = configuration["Authentication:JWT:Audience"];
        var secretKey = configuration["Authentication:JWT:SecretKey"];

        if (string.IsNullOrEmpty(secretKey))
        {
            throw new InvalidOperationException("JWT SecretKey is required");
        }

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(secretKey))
                };

                // Support API key authentication
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        // Check for API key in header (for service accounts)
                        var apiKey = context.Request.Headers["X-API-Key"].FirstOrDefault();
                        if (!string.IsNullOrEmpty(apiKey))
                        {
                            // TODO: Validate API key and generate claims
                        }
                        return Task.CompletedTask;
                    }
                };
            });

        return services;
    }

    private static IServiceCollection AddOidcAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var authority = configuration["Authentication:OIDC:Authority"];
        var clientId = configuration["Authentication:OIDC:ClientId"];

        services.AddAuthentication()
            .AddOpenIdConnect(options =>
            {
                options.Authority = authority;
                options.ClientId = clientId;
                options.ResponseType = "code";
                options.SaveTokens = true;
            });

        return services;
    }
}
