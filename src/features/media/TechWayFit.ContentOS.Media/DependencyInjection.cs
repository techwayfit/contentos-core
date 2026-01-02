using Microsoft.Extensions.DependencyInjection;
using TechWayFit.ContentOS.Media.Application;

namespace TechWayFit.ContentOS.Media;

public static class DependencyInjection
{
    public static IServiceCollection AddMediaServices(this IServiceCollection services)
    {
        // Register use cases
        services.AddScoped<IGetMediaMetadataUseCase, GetMediaMetadataUseCase>();

        // TODO: Add actual media storage service registration when implementing
        // services.AddScoped<IMediaStorageService, AzureBlobStorageService>();
        // or
        // services.AddScoped<IMediaStorageService, S3StorageService>();

        return services;
    }
}
