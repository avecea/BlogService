namespace BlogService.Domain.Repositories.Abstractions;

using BlogService.Domain.Entities;
using BlogService.Domain.Repositories.Abstractions.Base;

public interface ICommentRepository : IRepository<Comment, Guid>
{
    Task<IReadOnlyList<Comment>> GetByArticleIdAsync(Guid articleId, CancellationToken ct);
    Task<IReadOnlyList<Comment>> GetVisibleByArticleIdAsync(Guid articleId, CancellationToken ct);
}