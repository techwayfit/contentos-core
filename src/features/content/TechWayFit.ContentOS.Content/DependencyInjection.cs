using Microsoft.Extensions.DependencyInjection;
using TechWayFit.ContentOS.Content.Application;
using TechWayFit.ContentOS.Content.Application.ContentTypes;
using TechWayFit.ContentOS.Content.Application.ContentTypeFields;
using TechWayFit.ContentOS.Content.Application.ContentNodes;
using TechWayFit.ContentOS.Content.Application.Routes;
using TechWayFit.ContentOS.Content.Application.ContentItems;
using TechWayFit.ContentOS.Content.Application.ContentVersions;
using TechWayFit.ContentOS.Content.Application.ContentFieldValues;

namespace TechWayFit.ContentOS.Content;

public static class DependencyInjection
{
    public static IServiceCollection AddContent(this IServiceCollection services)
    {
        // Content Type use cases
        services.AddScoped<CreateContentTypeUseCase>();
        services.AddScoped<UpdateContentTypeUseCase>();
        services.AddScoped<GetContentTypeUseCase>();
        services.AddScoped<ListContentTypesUseCase>();
        services.AddScoped<DeleteContentTypeUseCase>();

        // Content Type Field use cases
        services.AddScoped<AddFieldToContentTypeUseCase>();
        services.AddScoped<UpdateContentTypeFieldUseCase>();
        services.AddScoped<RemoveFieldFromContentTypeUseCase>();
        services.AddScoped<ListContentTypeFieldsUseCase>();

        // Content Node use cases
        services.AddScoped<CreateContentNodeUseCase>();
        services.AddScoped<UpdateContentNodeUseCase>();
        services.AddScoped<GetContentNodeUseCase>();
        services.AddScoped<GetContentNodeChildrenUseCase>();
        services.AddScoped<DeleteContentNodeUseCase>();

        // Route use cases
        services.AddScoped<CreateRouteUseCase>();
        services.AddScoped<ResolveRouteUseCase>();
        services.AddScoped<ListRoutesForNodeUseCase>();
        services.AddScoped<DeleteRouteUseCase>();

        // Content Item use cases
        services.AddScoped<CreateContentItemUseCase>();
        services.AddScoped<UpdateContentItemUseCase>();
        services.AddScoped<GetContentItemUseCase>();
        services.AddScoped<ListContentItemsUseCase>();
        services.AddScoped<DeleteContentItemUseCase>();

        // Content Version use cases
        services.AddScoped<CreateContentVersionUseCase>();
        services.AddScoped<GetContentVersionUseCase>();
        services.AddScoped<ListContentVersionsUseCase>();
        services.AddScoped<GetLatestVersionUseCase>();
        services.AddScoped<GetPublishedVersionUseCase>();
        services.AddScoped<PublishContentVersionUseCase>();

        // Content Field Value use cases
        services.AddScoped<UpdateFieldValuesUseCase>();
        services.AddScoped<GetFieldValuesUseCase>();

        // Existing stub use cases (to be replaced)
        services.AddScoped<ICreateContentUseCase, CreateContentUseCase>();
        services.AddScoped<IAddLocalizationUseCase, AddLocalizationUseCase>();

        return services;
    }
}
