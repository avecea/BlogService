namespace BlogService.Application.Models.Comment;

using BlogService.Application.Models.Base;

public record class CreateCommentModel(
    Guid ArticleId,
    Guid AuthorId,
    string Text) : ICreateModel;
