namespace BlogService.Application.Models.Article;

using BlogService.Application.Models.Base;

public record class CreateArticleModel(
    Guid AuthorId,
    string Title,
    string Content) : ICreateModel;
