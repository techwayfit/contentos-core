using Microsoft.Extensions.DependencyInjection;

namespace TechWayFit.ContentOS.Search;

public static class DependencyInjection
{
    public static IServiceCollection AddSearchServices(this IServiceCollection services)
    {
        // Search feature domain services and ports
        // Event handlers and indexing implementations are in Infrastructure.Search

        // TODO: Add actual search service registration when implementing
        // services.AddScoped<ISearchService, ElasticsearchService>();

        return services;
    }
}
