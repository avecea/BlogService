namespace BlogService.Tests;

using AutoMapper;
using BlogService.Application.Models.Article;
using BlogService.Application.Models.Author;
using BlogService.Application.Models.Comment;
using BlogService.Application.Services;
using BlogService.Domain.Entities;
using BlogService.Domain.Enums;
using BlogService.Domain.Repositories.Abstractions;
using BlogService.Application.Services.Mapping;
using BlogService.Tests.Fakes;
using BlogService.ValueObjects;

public class ApplicationServicesTests
{
    private readonly InMemoryAuthorRepository _authors = new();
    private readonly InMemoryArticleRepository _articles = new();
    private readonly InMemoryCommentRepository _comments = new();
    private readonly IMapper _mapper;
    private readonly AuthorsApplicationService _authorService;
    private readonly ArticlesApplicationService _articleService;
    private readonly CommentsApplicationService _commentService;

    public ApplicationServicesTests()
    {
        _mapper = new MapperConfiguration(cfg => cfg.AddProfile<ApplicationProfile>()).CreateMapper();
        _authorService = new AuthorsApplicationService(_authors, _mapper);
        _articleService = new ArticlesApplicationService(_articles, _authors, _mapper);
        _commentService = new CommentsApplicationService(_comments, _articles, _authors, _mapper);
    }

    private async Task<Author> SeedAuthorAsync(Role role = Role.Author)
        => await _authors.AddAsync(
            new Author(Guid.NewGuid(), "Автор", "author@test.com", role), CancellationToken.None);

    private async Task<Article> SeedPublishedArticleAsync(Author author)
    {
        var article = author.CreateArticle(new Title("Статья"), new Content("Содержание"));
        article.Publish();
        return await _articles.AddAsync(article, CancellationToken.None);
    }

    [Fact]
    public async Task CreateAuthor_AddsAuthorAndReturnsModel()
    {
        var result = await _authorService.CreateModelAsync(
            new CreateAuthorModel("Иван", "ivan@test.com", Role.Author));

        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal("Иван", result.Name);
        Assert.Equal(Role.Author, result.Role);
        Assert.Single(await _authors.GetAllAsync(CancellationToken.None));
    }

    [Fact]
    public async Task CreateAuthor_WithDuplicateEmail_ReturnsNull()
    {
        await _authorService.CreateModelAsync(new CreateAuthorModel("Первый", "same@test.com"));

        var duplicate = await _authorService.CreateModelAsync(
            new CreateAuthorModel("Второй", "same@test.com"));

        Assert.Null(duplicate);
    }

    [Fact]
    public async Task GetAuthorByEmail_ReturnsAuthor()
    {
        var created = await _authorService.CreateModelAsync(
            new CreateAuthorModel("Иван", "ivan@test.com"));

        var found = await _authorService.GetAuthorByEmailAsync("ivan@test.com");

        Assert.NotNull(found);
        Assert.Equal(created!.Id, found.Id);
    }

    [Fact]
    public async Task UpdateAuthor_ChangesName()
    {
        var created = await _authorService.CreateModelAsync(
            new CreateAuthorModel("Старое имя", "a@test.com"));

        var updated = await _authorService.UpdateModelAsync(
            created! with { Name = "Новое имя" });

        var stored = await _authorService.GetModelByIdAsync(created!.Id);
        Assert.True(updated);
        Assert.Equal("Новое имя", stored!.Name);
    }

    [Fact]
    public async Task PromoteAuthor_ChangesRole()
    {
        var created = await _authorService.CreateModelAsync(
            new CreateAuthorModel("Читатель", "reader@test.com", Role.Reader));

        var result = await _authorService.PromoteToAuthorAsync(created!.Id);

        var stored = await _authorService.GetModelByIdAsync(created.Id);
        Assert.True(result);
        Assert.Equal(Role.Author, stored!.Role);
    }

    [Fact]
    public async Task DeleteAuthor_RemovesAuthor()
    {
        var created = await _authorService.CreateModelAsync(
            new CreateAuthorModel("Иван", "ivan@test.com"));

        var result = await _authorService.DeleteModelAsync(created!.Id);

        Assert.True(result);
        Assert.Empty(await _authors.GetAllAsync(CancellationToken.None));
    }

    [Fact]
    public async Task CreateArticle_MapsValueObjectsToStrings()
    {
        var author = await SeedAuthorAsync();

        var article = await _articleService.CreateModelAsync(
            new CreateArticleModel(author.Id, "Заголовок", "Текст статьи"));

        Assert.NotNull(article);
        Assert.Equal("Заголовок", article.Title);
        Assert.Equal("Текст статьи", article.Content);
        Assert.Equal(ArticleStatus.Draft, article.Status);
    }

    [Fact]
    public async Task CreateArticle_WithUnknownAuthor_ReturnsNull()
    {
        var result = await _articleService.CreateModelAsync(
            new CreateArticleModel(Guid.NewGuid(), "Заголовок", "Текст"));

        Assert.Null(result);
    }

    [Fact]
    public async Task GetArticlesByAuthor_ReturnsOnlyHisArticles()
    {
        var author = await SeedAuthorAsync();
        var other = await _authors.AddAsync(
            new Author(Guid.NewGuid(), "Другой", "other@test.com", Role.Author), CancellationToken.None);
        await _articleService.CreateModelAsync(new CreateArticleModel(author.Id, "Моя", "Текст"));
        await _articleService.CreateModelAsync(new CreateArticleModel(other.Id, "Чужая", "Текст"));

        var result = (await _articleService.GetArticlesByAuthorIdAsync(author.Id)).ToList();

        Assert.Single(result);
        Assert.Equal("Моя", result[0].Title);
    }

    [Fact]
    public async Task GetPublishedArticles_ReturnsOnlyPublished()
    {
        var author = await SeedAuthorAsync();
        await SeedPublishedArticleAsync(author);
        await _articleService.CreateModelAsync(new CreateArticleModel(author.Id, "Черновик", "Текст"));

        var result = await _articleService.GetPublishedArticlesAsync();

        Assert.Single(result);
        Assert.Equal("Статья", result.First().Title);
    }

    [Fact]
    public async Task PublishArticle_ChangesStatus()
    {
        var author = await SeedAuthorAsync();
        var created = await _articleService.CreateModelAsync(
            new CreateArticleModel(author.Id, "Статья", "Текст"));

        var result = await _articleService.PublishAsync(created!.Id);

        var stored = await _articleService.GetModelByIdAsync(created.Id);
        Assert.True(result);
        Assert.Equal(ArticleStatus.Published, stored!.Status);
        Assert.NotNull(stored.PublishedAt);
    }

    [Fact]
    public async Task ArchiveArticle_ChangesStatus()
    {
        var author = await SeedAuthorAsync();
        var article = await SeedPublishedArticleAsync(author);

        var result = await _articleService.ArchiveAsync(article.Id);

        var stored = await _articleService.GetModelByIdAsync(article.Id);
        Assert.True(result);
        Assert.Equal(ArticleStatus.Archived, stored!.Status);
    }

    [Fact]
    public async Task UpdateArticle_ChangesTitleAndContent()
    {
        var author = await SeedAuthorAsync();
        var created = await _articleService.CreateModelAsync(
            new CreateArticleModel(author.Id, "Старый заголовок", "Старый текст"));

        var result = await _articleService.UpdateModelAsync(
            created! with { Title = "Новый заголовок", Content = "Новый текст" });

        var stored = await _articleService.GetModelByIdAsync(created!.Id);
        Assert.True(result);
        Assert.Equal("Новый заголовок", stored!.Title);
        Assert.Equal("Новый текст", stored.Content);
    }

    [Fact]
    public async Task DeleteArticle_RemovesArticle()
    {
        var author = await SeedAuthorAsync();
        var created = await _articleService.CreateModelAsync(
            new CreateArticleModel(author.Id, "Статья", "Текст"));

        var result = await _articleService.DeleteModelAsync(created!.Id);

        Assert.True(result);
        Assert.Empty(await _articles.GetAllAsync(CancellationToken.None));
    }

    [Fact]
    public async Task CreateComment_MapsTextAndAddsComment()
    {
        var author = await SeedAuthorAsync();
        var article = await SeedPublishedArticleAsync(author);

        var comment = await _commentService.CreateModelAsync(
            new CreateCommentModel(article.Id, author.Id, "Отличная статья"));

        Assert.NotNull(comment);
        Assert.Equal("Отличная статья", comment.Text);
        Assert.Single(article.Comments);
    }

    [Fact]
    public async Task GetVisibleComments_ExcludesHidden()
    {
        var author = await SeedAuthorAsync();
        var article = await SeedPublishedArticleAsync(author);
        var first = await _commentService.CreateModelAsync(
            new CreateCommentModel(article.Id, author.Id, "Первый"));
        var second = await _commentService.CreateModelAsync(
            new CreateCommentModel(article.Id, author.Id, "Второй"));

        await _commentService.HideCommentAsync(first!.Id, author.Id);
        var visible = (await _commentService.GetVisibleCommentsByArticleIdAsync(article.Id)).ToList();
        var all = (await _commentService.GetCommentsByArticleIdAsync(article.Id)).ToList();

        Assert.Single(visible);
        Assert.Equal(second!.Id, visible[0].Id);
        Assert.Equal(2, all.Count);
    }

    [Fact]
    public async Task HideAndShowComment_ChangesVisibility()
    {
        var author = await SeedAuthorAsync();
        var article = await SeedPublishedArticleAsync(author);
        var comment = await _commentService.CreateModelAsync(
            new CreateCommentModel(article.Id, author.Id, "Текст"));

        var hidden = await _commentService.HideCommentAsync(comment!.Id, author.Id);
        Assert.True(hidden);
        Assert.True((await _commentService.GetModelByIdAsync(comment.Id))!.IsHidden);

        var shown = await _commentService.ShowCommentAsync(comment.Id, author.Id);
        Assert.True(shown);
        Assert.False((await _commentService.GetModelByIdAsync(comment.Id))!.IsHidden);
    }

    [Fact]
    public async Task UpdateComment_ChangesText()
    {
        var author = await SeedAuthorAsync();
        var article = await SeedPublishedArticleAsync(author);
        var comment = await _commentService.CreateModelAsync(
            new CreateCommentModel(article.Id, author.Id, "Старый текст"));

        var result = await _commentService.UpdateModelAsync(
            comment! with { Text = "Новый текст" });

        var stored = await _commentService.GetModelByIdAsync(comment!.Id);
        Assert.True(result);
        Assert.Equal("Новый текст", stored!.Text);
    }

    [Fact]
    public async Task DeleteComment_RemovesComment()
    {
        var author = await SeedAuthorAsync();
        var article = await SeedPublishedArticleAsync(author);
        var comment = await _commentService.CreateModelAsync(
            new CreateCommentModel(article.Id, author.Id, "Текст"));

        var result = await _commentService.DeleteModelAsync(comment!.Id);

        Assert.True(result);
        Assert.Empty(await _comments.GetAllAsync(CancellationToken.None));
    }
}
