using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechWayFit.ContentOS.Abstractions;
using TechWayFit.ContentOS.Contracts.Events;
using TechWayFit.ContentOS.Infrastructure.Search.EventHandlers;

namespace TechWayFit.ContentOS.Infrastructure.Search;

public static class DependencyInjection
{
    public static IServiceCollection AddSearch(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register event handlers for search indexing
        services.AddScoped<IEventHandler<ContentPublishedEventV1>, ContentPublishedIndexer>();

        var provider = configuration["Search:Provider"] ?? "Lucene";

        switch (provider)
        {
            case "Lucene":
                services.AddLuceneSearch(configuration);
                break;
            case "Azure":
                services.AddAzureSearch(configuration);
                break;
            case "OpenSearch":
                services.AddOpenSearch(configuration);
                break;
            default:
                throw new InvalidOperationException($"Unknown search provider: {provider}");
        }

        return services;
    }

    private static IServiceCollection AddLuceneSearch(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var indexPath = configuration["Search:Lucene:IndexPath"] ?? "./indexes";
        
        // services.AddSingleton<ISearchIndex, LuceneSearchIndex>(sp => 
        //     new LuceneSearchIndex(indexPath));
        
        return services;
    }

    private static IServiceCollection AddAzureSearch(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // services.AddSingleton<ISearchIndex, AzureSearchIndex>();
        return services;
    }

    private static IServiceCollection AddOpenSearch(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Future implementation
        throw new NotImplementedException("OpenSearch provider not yet implemented");
    }
}
