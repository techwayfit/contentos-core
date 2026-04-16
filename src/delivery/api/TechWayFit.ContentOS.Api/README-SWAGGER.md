# ContentOS API - Swagger Documentation

## Accessing the Swagger UI

### Development Environment

When running the API in Development mode, Swagger UI is automatically available at:

```
https://localhost:<port>/api/swagger
```

The Swagger JSON schema is available at:

```
https://localhost:<port>/api/swagger/v1/swagger.json
```

### Quick Start

1. **Start the API**
   ```bash
   cd src/delivery/api/TechWayFit.ContentOS.Api
   dotnet run
   ```

2. **Open Swagger UI**
   - Navigate to `https://localhost:5001/api/swagger` (or the port shown in console)
   - The interactive API documentation will load

3. **Set Required Headers**
   
   Most endpoints require multi-tenancy headers. Click "Authorize" or add headers directly:
   
   - **X-Tenant-Id**: Your tenant UUID (e.g., `550e8400-e29b-41d4-a716-446655440000`)
   - **X-Site-Id**: Your site UUID (e.g., `660e8400-e29b-41d4-a716-446655440001`)
   - **X-Environment**: `Draft` or `Published` (optional, defaults to `Published`)
   - **X-SuperAdmin**: `true` for admin endpoints (optional)

## Features

### ?? Interactive Documentation
- Browse all API endpoints organized by feature
- View request/response schemas with examples
- See detailed parameter descriptions
- Test endpoints directly from the browser

### ?? Security Schemes
- Header-based authentication (MVP)
- Clear documentation of required headers
- Permission requirements for each endpoint

### ?? Request Examples
- Sample JSON payloads for all operations
- Common use case examples
- Error response examples

### ?? Custom Branding
- TechWayFit branded color scheme
- Enhanced readability
- Professional presentation

## Common Workflows

### Creating Content

1. **POST /api/content**
   ```json
   {
     "contentType": "article",
     "languageCode": "en-US",
     "title": "My First Article",
     "slug": "my-first-article",
     "fields": {
 "body": "Content goes here...",
 "author": "John Doe"
     }
   }
   ```

2. **Add Localization**
   ```json
   POST /api/content/{contentId}/localizations
   {
     "languageCode": "es-ES",
 "title": "Mi Primer Artículo",
     "slug": "mi-primer-articulo",
     "fields": {
       "body": "El contenido va aquí...",
       "author": "Juan Pérez"
     }
   }
   ```

3. **Publish Content**
   ```json
   POST /api/content/{contentId}/workflow/transition
   {
     "targetState": "Published",
     "comment": "Ready for production"
   }
   ```

## API Organization

The API is organized into the following sections:

### System
- **GET /** - API information and health check

### Content
- **POST /api/content** - Create new content item
- **POST /api/content/{id}/localizations** - Add language variant
- **GET /api/content/{id}** - Retrieve content (planned)

### Workflow
- **POST /api/content/{contentId}/workflow/transition** - Change workflow state
- **GET /api/content/{contentId}/workflow** - Get workflow status (planned)

### Admin (Planned)
- Tenant management
- User management
- System configuration

## Response Codes

### Success
- **200 OK** - Request succeeded
- **201 Created** - Resource created successfully
- **204 No Content** - Request succeeded with no response body

### Client Errors
- **400 Bad Request** - Invalid input or validation error
- **401 Unauthorized** - Missing or invalid authentication
- **403 Forbidden** - Insufficient permissions
- **404 Not Found** - Resource does not exist
- **409 Conflict** - Resource conflict (e.g., duplicate slug)

### Server Errors
- **500 Internal Server Error** - Unexpected server error
- **503 Service Unavailable** - Service temporarily unavailable

## Error Response Format

All errors follow a consistent format:

```json
{
  "code": "VALIDATION_ERROR",
  "message": "One or more validation errors occurred",
  "validationErrors": {
    "title": ["Title is required"],
    "slug": ["Slug must be unique"]
  }
}
```

## Customization

### Custom Styles

The Swagger UI uses custom branding defined in:
```
wwwroot/swagger-ui/custom.css
```

To modify the appearance, edit the CSS variables:
```css
:root {
    --brand-primary: #0066cc;
    --brand-secondary: #004d99;
    --brand-accent: #00aaff;
}
```

### Adding Documentation

To add documentation to new endpoints:

1. **Add XML comments** to controller methods:
   ```csharp
   /// <summary>
   /// Brief description
   /// </summary>
   /// <remarks>
   /// Detailed description with examples
   /// </remarks>
   ```

2. **Use Swagger attributes**:
   ```csharp
   [SwaggerOperation(Summary = "...", Description = "...")]
   [SwaggerResponse(200, "Success", typeof(MyResponse))]
```

3. **Rebuild project** to regenerate XML documentation file

## Production Deployment

### Disabling Swagger in Production

By default, Swagger is only enabled in Development. To enable in other environments, modify `Program.cs`:

```csharp
// Current (Development only)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Alternative (All environments)
app.UseSwagger();
app.UseSwaggerUI();
```

### Securing Swagger in Production

If exposing Swagger in production, add authentication:

```csharp
app.UseSwagger(c => 
{
    c.PreSerializeFilters.Add((swagger, httpReq) =>
    {
   // Add authentication check
   if (!httpReq.Headers.ContainsKey("X-API-Key"))
 {
            throw new UnauthorizedAccessException();
        }
    });
});
```

## OpenAPI Specification

The OpenAPI 3.0 specification can be downloaded for use with:
- API clients (Postman, Insomnia)
- Code generation tools (OpenAPI Generator, NSwag)
- API testing frameworks
- Integration documentation

Download from: `https://localhost:<port>/api/swagger/v1/swagger.json`

## Troubleshooting

### Swagger UI Not Loading
- Ensure you're running in Development mode
- Check that `ASPNETCORE_ENVIRONMENT=Development`
- Verify the URL: `/api/swagger` (not `/swagger`)

### Missing XML Comments
- Ensure `<GenerateDocumentationFile>true</GenerateDocumentationFile>` is in `.csproj`
- Rebuild the project to regenerate XML file
- Check that XML file exists in `bin/Debug/net10.0/`

### Headers Not Working
- Use the "Authorize" button at the top of Swagger UI
- Or add headers manually in each request's "Parameters" section
- Headers must be valid UUIDs for Tenant/Site IDs

## Additional Resources

- [Swashbuckle Documentation](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)
- [OpenAPI Specification](https://swagger.io/specification/)
- [ContentOS Architecture Guide](../../docs/architecture.md)
- [API Design Principles](../../docs/api-design.md)

## Support

For issues or questions about the API documentation:
- Create an issue in the repository
- Contact the development team
- Check the architecture documentation
