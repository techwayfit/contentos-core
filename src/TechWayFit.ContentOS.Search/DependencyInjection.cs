using Microsoft.Extensions.DependencyInjection;
using TechWayFit.ContentOS.Abstractions;
using TechWayFit.ContentOS.Contracts.Events;
using TechWayFit.ContentOS.Search.Application;

namespace TechWayFit.ContentOS.Search;

public static class DependencyInjection
{
    public static IServiceCollection AddSearchServices(this IServiceCollection services)
    {
        // Register event handlers
        services.AddScoped<IEventHandler<ContentPublishedEvent>, ContentPublishedIndexer>();

        // TODO: Add actual search service registration when implementing
        // services.AddScoped<ISearchService, ElasticsearchService>();

        return services;
    }
}
