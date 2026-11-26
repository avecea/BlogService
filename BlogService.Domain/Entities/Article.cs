namespace BlogService.Domain.Entities;

using BlogService.Domain.Base;
using BlogService.Domain.Enums;
using BlogService.Domain.Exceptions;
using BlogService.ValueObjects;

public class Article : Entity<Guid>
{
    private readonly List<Comment> _comments = new();

    public Guid AuthorId { get; private set; }
    public Author Author { get; private set; } = default!;
    public Title Title { get; private set; } = default!;
    public Content Content { get; private set; } = default!;
    public ArticleStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public DateTime? PublishedAt { get; private set; }

    public IReadOnlyCollection<Comment> Comments => _comments.AsReadOnly();

    protected Article() { }

    public Article(Guid id, Author author, Title title, Content content) : base(id)
    {
        Author = author ?? throw new ArgumentNullException(nameof(author));
        AuthorId = author.Id;
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Content = content ?? throw new ArgumentNullException(nameof(content));
        Status = ArticleStatus.Draft;
        CreatedAt = DateTime.UtcNow;
    }

    public void Update(Title title, Content content)
    {
        if (Status == ArticleStatus.Published)
            throw new ValidationException(nameof(Status), "Опубликованную статью нельзя редактировать");

        Title = title ?? throw new ArgumentNullException(nameof(title));
        Content = content ?? throw new ArgumentNullException(nameof(content));
        UpdatedAt = DateTime.UtcNow;
    }

    public void Publish()
    {
        if (Status != ArticleStatus.Draft)
            throw new ValidationException(nameof(Status), "Можно опубликовать только черновик");

        Status = ArticleStatus.Published;
        PublishedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Archive()
    {
        if (Status != ArticleStatus.Published)
            throw new ValidationException(nameof(Status), "Можно архивировать только опубликованную статью");

        Status = ArticleStatus.Archived;
        UpdatedAt = DateTime.UtcNow;
    }

    public Comment AddComment(CommentText text, Author author)
    {
        if (Status != ArticleStatus.Published)
            throw new ValidationException(nameof(Status), "Комментарии только к опубликованным статьям");

        var comment = new Comment(Guid.NewGuid(), this, author, text);
        _comments.Add(comment);
        return comment;
    }

    public void HideComment(Comment comment, Author requester)
    {
        if (comment == null)
            throw new ArgumentNullException(nameof(comment));

        if (comment.ArticleId != Id)
            throw new ValidationException(nameof(comment), "Комментарий не принадлежит этой статье");

        if (requester.Id != AuthorId)
            throw new UnauthorizedActionException("Только автор статьи может скрыть комментарий");

        comment.Hide();
    }

    public void ShowComment(Comment comment, Author requester)
    {
        if (comment == null)
            throw new ArgumentNullException(nameof(comment));

        if (comment.ArticleId != Id)
            throw new ValidationException(nameof(comment), "Комментарий не принадлежит этой статье");

        if (requester.Id != AuthorId)
            throw new UnauthorizedActionException("Только автор статьи может показать комментарий");

        comment.Show();
    }
}