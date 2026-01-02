using Microsoft.AspNetCore.Mvc;

namespace TechWayFit.ContentOS.Api.Controllers;

/// <summary>
/// Health check endpoint for API monitoring
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new 
        { 
            Status = "Healthy",
            Timestamp = DateTimeOffset.UtcNow
        });
    }
}
