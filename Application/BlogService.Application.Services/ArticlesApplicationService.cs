namespace BlogService.Application.Services;

using AutoMapper;
using BlogService.Application.Models.Article;
using BlogService.Application.Services.Abstractions;
using BlogService.Domain.Repositories.Abstractions;
using BlogService.ValueObjects;

public class ArticlesApplicationService(
    IArticleRepository articleRepository,
    IAuthorRepository authorRepository,
    IMapper mapper) : IArticlesApplicationService
{
    public async Task<IEnumerable<ArticleModel>> GetModelsAsync(CancellationToken cancellationToken = default)
        => (await articleRepository.GetAllAsync(cancellationToken)).Select(mapper.Map<ArticleModel>);

    public async Task<ArticleModel?> GetModelByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var article = await articleRepository.GetByIdAsync(id, cancellationToken);
        return article is null ? null : mapper.Map<ArticleModel>(article);
    }

    public async Task<IEnumerable<ArticleModel>> GetArticlesByAuthorIdAsync(Guid authorId, CancellationToken cancellationToken = default)
        => (await articleRepository.GetByAuthorIdAsync(authorId, cancellationToken)).Select(mapper.Map<ArticleModel>);

    public async Task<IEnumerable<ArticleModel>> GetPublishedArticlesAsync(CancellationToken cancellationToken = default)
        => (await articleRepository.GetPublishedAsync(cancellationToken)).Select(mapper.Map<ArticleModel>);

    public async Task<ArticleModel?> CreateModelAsync(CreateArticleModel model, CancellationToken cancellationToken = default)
    {
        var author = await authorRepository.GetByIdAsync(model.AuthorId, cancellationToken);
        if (author is null)
            return null;

        var article = author.CreateArticle(new Title(model.Title), new Content(model.Content));
        var createdArticle = await articleRepository.AddAsync(article, cancellationToken);
        return mapper.Map<ArticleModel>(createdArticle);
    }

    public async Task<bool> UpdateModelAsync(ArticleModel model, CancellationToken cancellationToken = default)
    {
        var article = await articleRepository.GetByIdAsync(model.Id, cancellationToken);
        if (article is null)
            return false;

        article.Update(new Title(model.Title), new Content(model.Content));
        await articleRepository.UpdateAsync(article, cancellationToken);
        return true;
    }

    public async Task<bool> DeleteModelAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var article = await articleRepository.GetByIdAsync(id, cancellationToken);
        if (article is null)
            return false;

        await articleRepository.DeleteAsync(article, cancellationToken);
        return true;
    }

    public async Task<bool> PublishAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var article = await articleRepository.GetByIdAsync(id, cancellationToken);
        if (article is null)
            return false;

        article.Publish();
        await articleRepository.UpdateAsync(article, cancellationToken);
        return true;
    }

    public async Task<bool> ArchiveAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var article = await articleRepository.GetByIdAsync(id, cancellationToken);
        if (article is null)
            return false;

        article.Archive();
        await articleRepository.UpdateAsync(article, cancellationToken);
        return true;
    }
}
