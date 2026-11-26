namespace BlogService.Domain.Entities;

using BlogService.Domain.Base;
using BlogService.Domain.Enums;
using BlogService.Domain.Exceptions;
using BlogService.ValueObjects;

public class Author : Entity<Guid>
{
    private readonly List<Article> _articles = new();

    public string Name { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public Role Role { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public IReadOnlyCollection<Article> Articles => _articles.AsReadOnly();

    protected Author() { }

    public Author(Guid id, string name, string email, Role role = Role.Reader) : base(id)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ValidationException(nameof(name), "Имя обязательно");
        if (string.IsNullOrWhiteSpace(email))
            throw new ValidationException(nameof(email), "Email обязателен");

        Name = name;
        Email = email;
        Role = role;
        CreatedAt = DateTime.UtcNow;
    }

    public void Update(string name, string email)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ValidationException(nameof(name), "Имя обязательно");
        if (string.IsNullOrWhiteSpace(email))
            throw new ValidationException(nameof(email), "Email обязателен");

        Name = name;
        Email = email;
    }

    public Article CreateArticle(Title title, Content content)
    {
        if (Role != Role.Author)
            throw new UnauthorizedActionException("Только автор может создавать статьи");

        var article = new Article(Guid.NewGuid(), this, title, content);
        _articles.Add(article);
        return article;
    }

    public void DeleteArticle(Article article)
    {
        if (article == null)
            throw new ArgumentNullException(nameof(article));
        if (article.AuthorId != Id)
            throw new UnauthorizedActionException("Только автор может удалить статью");

        _articles.Remove(article);
    }

    public void PromoteToAuthor()
    {
        Role = Role.Author;
    }
}