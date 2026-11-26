namespace BlogService.Tests.Fakes;

using BlogService.Domain.Entities;
using BlogService.Domain.Repositories.Abstractions;

public sealed class InMemoryAuthorRepository : IAuthorRepository
{
    private readonly Dictionary<Guid, Author> _items = new();

    public Task<Author?> GetByIdAsync(Guid id, CancellationToken ct)
        => Task.FromResult(_items.GetValueOrDefault(id));

    public Task<IReadOnlyList<Author>> GetAllAsync(CancellationToken ct)
        => Task.FromResult<IReadOnlyList<Author>>(_items.Values.ToList());

    public Task<Author> AddAsync(Author entity, CancellationToken ct)
    {
        _items[entity.Id] = entity;
        return Task.FromResult(entity);
    }

    public Task UpdateAsync(Author entity, CancellationToken ct)
    {
        _items[entity.Id] = entity;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Author entity, CancellationToken ct)
    {
        _items.Remove(entity.Id);
        return Task.CompletedTask;
    }

    public Task<Author?> GetByEmailAsync(string email, CancellationToken ct)
        => Task.FromResult(_items.Values.FirstOrDefault(a => a.Email == email));
}

public sealed class InMemoryArticleRepository : IArticleRepository
{
    private readonly Dictionary<Guid, Article> _items = new();

    public Task<Article?> GetByIdAsync(Guid id, CancellationToken ct)
        => Task.FromResult(_items.GetValueOrDefault(id));

    public Task<IReadOnlyList<Article>> GetAllAsync(CancellationToken ct)
        => Task.FromResult<IReadOnlyList<Article>>(_items.Values.ToList());

    public Task<Article> AddAsync(Article entity, CancellationToken ct)
    {
        _items[entity.Id] = entity;
        return Task.FromResult(entity);
    }

    public Task UpdateAsync(Article entity, CancellationToken ct)
    {
        _items[entity.Id] = entity;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Article entity, CancellationToken ct)
    {
        _items.Remove(entity.Id);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<Article>> GetByAuthorIdAsync(Guid authorId, CancellationToken ct)
        => Task.FromResult<IReadOnlyList<Article>>(_items.Values.Where(a => a.AuthorId == authorId).ToList());

    public Task<IReadOnlyList<Article>> GetPublishedAsync(CancellationToken ct)
        => Task.FromResult<IReadOnlyList<Article>>(
            _items.Values.Where(a => a.Status == Domain.Enums.ArticleStatus.Published).ToList());
}

public sealed class InMemoryCommentRepository : ICommentRepository
{
    private readonly Dictionary<Guid, Comment> _items = new();

    public Task<Comment?> GetByIdAsync(Guid id, CancellationToken ct)
        => Task.FromResult(_items.GetValueOrDefault(id));

    public Task<IReadOnlyList<Comment>> GetAllAsync(CancellationToken ct)
        => Task.FromResult<IReadOnlyList<Comment>>(_items.Values.ToList());

    public Task<Comment> AddAsync(Comment entity, CancellationToken ct)
    {
        _items[entity.Id] = entity;
        return Task.FromResult(entity);
    }

    public Task UpdateAsync(Comment entity, CancellationToken ct)
    {
        _items[entity.Id] = entity;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Comment entity, CancellationToken ct)
    {
        _items.Remove(entity.Id);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<Comment>> GetByArticleIdAsync(Guid articleId, CancellationToken ct)
        => Task.FromResult<IReadOnlyList<Comment>>(_items.Values.Where(c => c.ArticleId == articleId).ToList());

    public Task<IReadOnlyList<Comment>> GetVisibleByArticleIdAsync(Guid articleId, CancellationToken ct)
        => Task.FromResult<IReadOnlyList<Comment>>(
            _items.Values.Where(c => c.ArticleId == articleId && !c.IsHidden).ToList());
}
