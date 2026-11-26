namespace BlogService.Application.Services.Abstractions;

using BlogService.Application.Models.Author;
using BlogService.Application.Services.Abstractions.Base;

public interface IAuthorsApplicationService : IApplicationService<AuthorModel, CreateAuthorModel, Guid>
{
    Task<AuthorModel?> GetAuthorByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<bool> PromoteToAuthorAsync(Guid id, CancellationToken cancellationToken = default);
}
