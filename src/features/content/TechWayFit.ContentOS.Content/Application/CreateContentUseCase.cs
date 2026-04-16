using TechWayFit.ContentOS.Content.Domain;
using TechWayFit.ContentOS.Contracts.Dtos;
using TechWayFit.ContentOS.Kernel;

namespace TechWayFit.ContentOS.Content.Application;

/// <summary>
/// Use case implementation for creating new content
/// </summary>
public class CreateContentUseCase : ICreateContentUseCase
{
    public Task<Result<ContentResponse, Error>> ExecuteAsync(
        CreateContentCommand command,
        CancellationToken cancellationToken = default)
    {
        // TODO: Implement content creation logic
  // This is a stub implementation to fix build errors
        var error = new Error
        {
 Code = "NotImplemented",
        Message = "CreateContent use case not yet implemented"
      };
        return Task.FromResult(Result.Fail<ContentResponse, Error>(error));
    }
}
