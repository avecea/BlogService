namespace BlogService.Application.Models.Comment;

using BlogService.Application.Models.Base;

public record class CommentModel(
    Guid Id,
    Guid ArticleId,
    Guid AuthorId,
    string Text,
    bool IsHidden,
    DateTime CreatedAt) : IModel<Guid>;
