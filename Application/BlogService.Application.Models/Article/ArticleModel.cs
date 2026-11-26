namespace BlogService.Application.Models.Article;

using BlogService.Application.Models.Base;
using BlogService.Domain.Enums;

public record class ArticleModel(
    Guid Id,
    Guid AuthorId,
    string Title,
    string Content,
    ArticleStatus Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    DateTime? PublishedAt) : IModel<Guid>;
