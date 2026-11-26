namespace BlogService.Domain.Repositories.Abstractions;

using BlogService.Domain.Entities;
using BlogService.Domain.Repositories.Abstractions.Base;

public interface IAuthorRepository : IRepository<Author, Guid>
{
    Task<Author?> GetByEmailAsync(string email, CancellationToken ct);
}