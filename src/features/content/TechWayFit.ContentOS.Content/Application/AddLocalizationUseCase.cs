using TechWayFit.ContentOS.Content.Domain;
using TechWayFit.ContentOS.Contracts.Dtos;
using TechWayFit.ContentOS.Kernel;

namespace TechWayFit.ContentOS.Content.Application;

/// <summary>
/// Use case implementation for adding localization to content
/// </summary>
public class AddLocalizationUseCase : IAddLocalizationUseCase
{
    public Task<Result<ContentResponse, Error>> ExecuteAsync(
        AddLocalizationCommand command,
        CancellationToken cancellationToken = default)
    {
        // TODO: Implement localization logic
// This is a stub implementation to fix build errors
        var error = new Error
        {
            Code = "NotImplemented",
            Message = "AddLocalization use case not yet implemented"
        };
        return Task.FromResult(Result.Fail<ContentResponse, Error>(error));
    }
}
