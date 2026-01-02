using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using TechWayFit.ContentOS.Contracts.Dtos;
using TechWayFit.ContentOS.Infrastructure.Persistence.Postgres;
using Xunit;

namespace TechWayFit.ContentOS.Api.Tests;

/// <summary>
/// Integration test demonstrating full vertical slice:
/// Create Draft → Transition to Review → Publish → Verify events fired
/// </summary>
public class ContentWorkflowIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public ContentWorkflowIntegrationTests(WebApplicationFactory<Program> factory)
    {
        // Override with in-memory database for testing
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove the existing DbContext registration
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<PostgresDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add in-memory database
                services.AddDbContext<PostgresDbContext>(options =>
                {
                    options.UseInMemoryDatabase("IntegrationTestDb");
                });
                
                // Register ContentOsDbContext alias
                services.AddScoped<ContentOsDbContext>(sp => sp.GetRequiredService<PostgresDbContext>());
            });
        });

        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task CreateContent_TransitionToReview_Publish_ShouldSucceed()
    {
        // Arrange
        var createRequest = new CreateContentRequest
        {
            ContentType = "article",
            LanguageCode = "en-US",
            Title = "Test Article",
            Slug = "test-article",
            Fields = new Dictionary<string, object>
            {
                ["body"] = "This is a test article body",
                ["author"] = "Test Author"
            }
        };

        // Act 1: Create content (Draft state)
        var createResponse = await _client.PostAsJsonAsync("/api/content", createRequest);
        createResponse.EnsureSuccessStatusCode();
        var content = await createResponse.Content.ReadFromJsonAsync<ContentResponse>();

        Assert.NotNull(content);
        Assert.NotEqual(Guid.Empty, content.Id);
        Assert.Equal("Draft", content.WorkflowStatus);

        // Act 2: Transition to InReview
        var reviewRequest = new WorkflowTransitionRequest
        {
            ContentId = content.Id,
            TargetState = "InReview",
            Comment = "Ready for review"
        };

        var reviewResponse = await _client.PostAsJsonAsync(
            $"/api/content/{content.Id}/workflow/transition",
            reviewRequest);
        reviewResponse.EnsureSuccessStatusCode();
        var reviewState = await reviewResponse.Content.ReadFromJsonAsync<WorkflowStateResponse>();

        Assert.NotNull(reviewState);
        Assert.Equal("InReview", reviewState.CurrentStatus);

        // Act 3: Publish
        var publishRequest = new WorkflowTransitionRequest
        {
            ContentId = content.Id,
            TargetState = "Published",
            Comment = "Approved for publication"
        };

        var publishResponse = await _client.PostAsJsonAsync(
            $"/api/content/{content.Id}/workflow/transition",
            publishRequest);
        publishResponse.EnsureSuccessStatusCode();
        var publishState = await publishResponse.Content.ReadFromJsonAsync<WorkflowStateResponse>();

        // Assert
        Assert.NotNull(publishState);
        Assert.Equal("Published", publishState.CurrentStatus);
        Assert.NotNull(publishState.TransitionedAt);
        
        // TODO: Add event verification when event bus exposes subscribed handlers
        // For now, check console logs for:
        // - ContentCreatedEvent
        // - WorkflowTransitionedEvent (Draft → InReview)
        // - WorkflowTransitionedEvent (InReview → Published)
        // - ContentPublishedEvent
        // - ContentPublishedIndexer logging
    }

    [Fact]
    public async Task AddLocalization_ShouldCreateAdditionalLanguageVersion()
    {
        // Arrange
        var createRequest = new CreateContentRequest
        {
            ContentType = "article",
            LanguageCode = "en-US",
            Title = "English Article",
            Slug = "english-article",
            Fields = new Dictionary<string, object> { ["body"] = "English content" }
        };

        var createResponse = await _client.PostAsJsonAsync("/api/content", createRequest);
        var content = await createResponse.Content.ReadFromJsonAsync<ContentResponse>();

        var localizationRequest = new AddLocalizationRequest
        {
            LanguageCode = "es-ES",
            Title = "Artículo en Español",
            Slug = "articulo-espanol",
            Fields = new Dictionary<string, object> { ["body"] = "Contenido en español" }
        };

        // Act
        var localizationResponse = await _client.PostAsJsonAsync(
            $"/api/content/{content!.Id}/localizations",
            localizationRequest);
        localizationResponse.EnsureSuccessStatusCode();

        // Assert
        var localizedContent = await localizationResponse.Content.ReadFromJsonAsync<ContentResponse>();
        Assert.NotNull(localizedContent);
        // TODO: Verify localization count when GET endpoint is implemented
    }

    [Fact]
    public async Task GetMediaMetadata_ShouldReturnStubData()
    {
        // Arrange
        var mediaId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/media/{mediaId}");
        response.EnsureSuccessStatusCode();

        var metadata = await response.Content.ReadFromJsonAsync<MediaMetadataResponse>();

        // Assert
        Assert.NotNull(metadata);
        Assert.Equal(mediaId, metadata.MediaId);
        Assert.Equal("sample-image.jpg", metadata.FileName);
    }
}
