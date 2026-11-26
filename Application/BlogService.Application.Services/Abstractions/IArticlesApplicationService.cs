namespace BlogService.Application.Services.Abstractions;

using BlogService.Application.Models.Article;
using BlogService.Application.Services.Abstractions.Base;

public interface IArticlesApplicationService : IApplicationService<ArticleModel, CreateArticleModel, Guid>
{
    Task<IEnumerable<ArticleModel>> GetArticlesByAuthorIdAsync(Guid authorId, CancellationToken cancellationToken = default);

    Task<IEnumerable<ArticleModel>> GetPublishedArticlesAsync(CancellationToken cancellationToken = default);

    Task<bool> PublishAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> ArchiveAsync(Guid id, CancellationToken cancellationToken = default);
}
