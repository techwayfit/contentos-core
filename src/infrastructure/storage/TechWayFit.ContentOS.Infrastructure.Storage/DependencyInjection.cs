using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TechWayFit.ContentOS.Infrastructure.Storage;

public static class DependencyInjection
{
    public static IServiceCollection AddBlobStorage(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var provider = configuration["BlobStorage:Provider"] ?? "LocalFileSystem";

        switch (provider)
        {
            case "LocalFileSystem":
                services.AddLocalFileSystemStorage(configuration);
                break;
            case "Azure":
                services.AddAzureBlobStorage(configuration);
                break;
            case "S3":
                services.AddS3BlobStorage(configuration);
                break;
            default:
                throw new InvalidOperationException($"Unknown blob storage provider: {provider}");
        }

        return services;
    }

    private static IServiceCollection AddLocalFileSystemStorage(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // services.AddSingleton<IBlobStore, LocalFileSystemBlobStore>();
        return services;
    }

    private static IServiceCollection AddAzureBlobStorage(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // services.AddSingleton<IBlobStore, AzureBlobStore>();
        return services;
    }

    private static IServiceCollection AddS3BlobStorage(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // services.AddSingleton<IBlobStore, S3BlobStore>();
        return services;
    }
}
