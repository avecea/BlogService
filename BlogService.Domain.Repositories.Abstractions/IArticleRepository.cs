namespace BlogService.Domain.Repositories.Abstractions;

using BlogService.Domain.Entities;
using BlogService.Domain.Repositories.Abstractions.Base;

public interface IArticleRepository : IRepository<Article, Guid>
{
    Task<IReadOnlyList<Article>> GetByAuthorIdAsync(Guid authorId, CancellationToken ct);
    Task<IReadOnlyList<Article>> GetPublishedAsync(CancellationToken ct);
}