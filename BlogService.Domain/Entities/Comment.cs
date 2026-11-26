namespace BlogService.Domain.Entities;

using BlogService.Domain.Base;
using BlogService.Domain.Exceptions;
using BlogService.ValueObjects;

public class Comment : Entity<Guid>
{
    public Guid ArticleId { get; private set; }
    public Article Article { get; private set; } = default!;
    public Guid AuthorId { get; private set; }
    public Author Author { get; private set; } = default!;
    public CommentText Text { get; private set; } = default!;
    public bool IsHidden { get; private set; }
    public DateTime CreatedAt { get; private set; }

    protected Comment() { }

    public Comment(Guid id, Article article, Author author, CommentText text) : base(id)
    {
        Article = article ?? throw new ArgumentNullException(nameof(article));
        ArticleId = article.Id;
        Author = author ?? throw new ArgumentNullException(nameof(author));
        AuthorId = author.Id;
        Text = text ?? throw new ArgumentNullException(nameof(text));
        IsHidden = false;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateText(CommentText newText)
    {
        Text = newText ?? throw new ArgumentNullException(nameof(newText));
    }

    public void Hide()
    {
        IsHidden = true;
    }

    public void Show()
    {
        IsHidden = false;
    }
}