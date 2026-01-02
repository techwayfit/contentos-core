using TechWayFit.ContentOS.Content.Domain;
using TechWayFit.ContentOS.Contracts.Dtos;
using TechWayFit.ContentOS.Kernel;

namespace TechWayFit.ContentOS.Content.Application;

/// <summary>
/// Command for adding localization to content
/// </summary>
public record AddLocalizationCommand(
    ContentItemId ContentId,
    string LanguageCode,
    string Title,
    string Slug,
    Dictionary<string, object> Fields
);

/// <summary>
/// Use case for adding localization to existing content
/// </summary>
public interface IAddLocalizationUseCase
{
    Task<Result<ContentResponse, Error>> ExecuteAsync(
        AddLocalizationCommand command,
        CancellationToken cancellationToken = default);
}
