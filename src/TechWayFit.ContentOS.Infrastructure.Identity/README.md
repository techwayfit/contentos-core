# TechWayFit.ContentOS.Infrastructure.Identity

## Purpose
Concrete implementations of authentication and authorization.

## Responsibilities
- **JWT Token Validation**: Validate bearer tokens
- **OpenID Connect**: Integration with OIDC providers (Auth0, Okta, Azure AD, etc.)
- **Claims Transformation**: Map external claims to ContentOS permissions
- **Token Generation**: Issue tokens for API access
- **Multi-tenancy**: Extract and validate tenant context from auth

## Key Principles
- **Standards-based**: Use OAuth 2.0/OIDC standards
- **Provider-agnostic**: Support multiple identity providers
- **Policy-driven**: Authorization based on Kernel policies
- **Secure by default**: Deny-by-default, validate all tokens

## Structure
```
Services/
  JwtTokenValidator.cs
  ClaimsTransformer.cs
  TokenGenerator.cs
Providers/
  OidcAuthenticationProvider.cs
  JwtAuthenticationProvider.cs
DependencyInjection.cs
```

## Usage
In API/Program.cs:
```csharp
// Option 1: JWT Bearer
services.AddJwtAuthentication(configuration);

// Option 2: OpenID Connect
services.AddOidcAuthentication(configuration);
```

## Configuration
```json
{
  "Authentication": {
    "Provider": "JWT", // or "OIDC"
    "JWT": {
      "Issuer": "https://contentos.example.com",
      "Audience": "contentos-api",
      "SecretKey": "..."
    },
    "OIDC": {
      "Authority": "https://login.microsoftonline.com/tenant-id",
      "ClientId": "...",
      "ClientSecret": "..."
    }
  }
}
```

## Features
- Bearer token authentication
- Role-based access control (RBAC)
- Permission-based authorization
- Tenant context extraction
- API key authentication (optional)
- Social login integration (future)

## Security Considerations
- Never log tokens or secrets
- Validate issuer, audience, signature
- Use HTTPS in production
- Rotate keys regularly
- Implement token revocation

## Dependencies
- Abstractions (ICurrentUser, etc.)
- Kernel (permission model)
- Microsoft.AspNetCore.Authentication.JwtBearer
- Microsoft.AspNetCore.Authentication.OpenIdConnect
