using TechWayFit.ContentOS.Content.Domain;
using TechWayFit.ContentOS.Contracts.Dtos;
using TechWayFit.ContentOS.Kernel;

namespace TechWayFit.ContentOS.Content.Application;

/// <summary>
/// Command for creating content
/// </summary>
public record CreateContentCommand(
    string ContentType,
    string LanguageCode,
    string Title,
    string Slug,
    Dictionary<string, object> Fields
);

/// <summary>
/// Use case for creating new content
/// </summary>
public interface ICreateContentUseCase
{
    Task<Result<ContentResponse, Error>> ExecuteAsync(
        CreateContentCommand command,
        CancellationToken cancellationToken = default);
}
