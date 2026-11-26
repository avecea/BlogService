namespace BlogService.Tests;

using BlogService.Domain.Entities;
using BlogService.Domain.Enums;
using BlogService.Domain.Exceptions;
using BlogService.ValueObjects;

public class DomainTests
{
    private static Author NewAuthor(Role role = Role.Author)
        => new(Guid.NewGuid(), "Тестовый автор", "author@test.com", role);

    private static Article NewPublishedArticle(out Author author)
    {
        author = NewAuthor();
        var article = author.CreateArticle(new Title("Заголовок"), new Content("Текст статьи"));
        article.Publish();
        return article;
    }

    [Fact]
    public void Title_WithValidValue_StoresValue()
    {
        var title = new Title("Моя статья");

        Assert.Equal("Моя статья", title.Value);
    }

    [Fact]
    public void Title_WithEmptyValue_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Title("   "));
    }

    [Fact]
    public void Title_TooLong_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Title(new string('a', 201)));
    }

    [Fact]
    public void Content_WithEmptyValue_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Content(""));
    }

    [Fact]
    public void CommentText_TooLong_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new CommentText(new string('x', 1001)));
    }

    [Fact]
    public void ValueObjects_WithSameValue_AreEqual()
    {
        Assert.Equal(new Title("Одинаково"), new Title("Одинаково"));
        Assert.Equal(new Title("Одинаково").GetHashCode(), new Title("Одинаково").GetHashCode());
    }

    [Fact]
    public void Author_WithEmptyName_ThrowsValidationException()
    {
        Assert.Throws<ValidationException>(() => new Author(Guid.NewGuid(), "", "a@b.com", Role.Author));
    }

    [Fact]
    public void Author_Update_ChangesNameAndEmail()
    {
        var author = NewAuthor();

        author.Update("Новое имя", "new@test.com");

        Assert.Equal("Новое имя", author.Name);
        Assert.Equal("new@test.com", author.Email);
    }

    [Fact]
    public void Author_AsReader_CannotCreateArticle()
    {
        var reader = NewAuthor(Role.Reader);

        Assert.Throws<UnauthorizedActionException>(() =>
            reader.CreateArticle(new Title("T"), new Content("C")));
    }

    [Fact]
    public void Author_AfterPromote_CanCreateArticle()
    {
        var author = NewAuthor(Role.Reader);
        author.PromoteToAuthor();

        var article = author.CreateArticle(new Title("T"), new Content("C"));

        Assert.Equal(Role.Author, author.Role);
        Assert.Single(author.Articles);
        Assert.Equal(article.Id, author.Articles.First().Id);
    }

    [Fact]
    public void Article_WhenCreated_IsDraft()
    {
        var article = NewPublishedArticle(out _);
        var draft = NewAuthor().CreateArticle(new Title("Черновик"), new Content("Текст"));

        Assert.Equal(ArticleStatus.Draft, draft.Status);
        Assert.Null(draft.PublishedAt);
        Assert.Equal(ArticleStatus.Published, article.Status);
        Assert.NotNull(article.PublishedAt);
    }

    [Fact]
    public void Article_PublishTwice_ThrowsValidationException()
    {
        var article = NewPublishedArticle(out _);

        Assert.Throws<ValidationException>(() => article.Publish());
    }

    [Fact]
    public void Article_ArchiveFromDraft_ThrowsValidationException()
    {
        var draft = NewAuthor().CreateArticle(new Title("T"), new Content("C"));

        Assert.Throws<ValidationException>(() => draft.Archive());
    }

    [Fact]
    public void Article_ArchivePublished_BecomesArchived()
    {
        var article = NewPublishedArticle(out _);

        article.Archive();

        Assert.Equal(ArticleStatus.Archived, article.Status);
        Assert.NotNull(article.UpdatedAt);
    }

    [Fact]
    public void Article_UpdatePublished_ThrowsValidationException()
    {
        var article = NewPublishedArticle(out _);

        Assert.Throws<ValidationException>(() =>
            article.Update(new Title("Новый заголовок"), new Content("Новый текст")));
    }

    [Fact]
    public void Article_UpdateDraft_ChangesContentAndSetsUpdatedAt()
    {
        var article = NewAuthor().CreateArticle(new Title("Старый"), new Content("Текст"));

        article.Update(new Title("Новый"), new Content("Обновлённый текст"));

        Assert.Equal("Новый", article.Title.Value);
        Assert.Equal("Обновлённый текст", article.Content.Value);
        Assert.NotNull(article.UpdatedAt);
    }

    [Fact]
    public void Article_AddCommentToDraft_ThrowsValidationException()
    {
        var draft = NewAuthor().CreateArticle(new Title("T"), new Content("C"));

        Assert.Throws<ValidationException>(() =>
            draft.AddComment(new CommentText("Комментарий"), NewAuthor()));
    }

    [Fact]
    public void Article_AddCommentToPublished_AddsComment()
    {
        var article = NewPublishedArticle(out _);

        var comment = article.AddComment(new CommentText("Хорошая статья"), NewAuthor());

        Assert.Single(article.Comments);
        Assert.False(comment.IsHidden);
        Assert.Equal(article.Id, comment.ArticleId);
    }

    [Fact]
    public void Article_HideCommentByOtherAuthor_ThrowsUnauthorizedActionException()
    {
        var article = NewPublishedArticle(out _);
        var comment = article.AddComment(new CommentText("Текст"), NewAuthor());
        var stranger = NewAuthor();

        Assert.Throws<UnauthorizedActionException>(() => article.HideComment(comment, stranger));
    }

    [Fact]
    public void Article_HideAndShowCommentByOwner_ChangesVisibility()
    {
        var article = NewPublishedArticle(out var owner);
        var comment = article.AddComment(new CommentText("Текст"), NewAuthor());

        article.HideComment(comment, owner);
        Assert.True(comment.IsHidden);

        article.ShowComment(comment, owner);
        Assert.False(comment.IsHidden);
    }

    [Fact]
    public void Article_HideCommentFromOtherArticle_ThrowsValidationException()
    {
        var article = NewPublishedArticle(out var owner);
        var otherArticle = NewPublishedArticle(out _);
        var foreignComment = otherArticle.AddComment(new CommentText("Текст"), NewAuthor());

        Assert.Throws<ValidationException>(() => article.HideComment(foreignComment, owner));
    }

    [Fact]
    public void Comment_UpdateText_ChangesText()
    {
        var article = NewPublishedArticle(out _);
        var comment = article.AddComment(new CommentText("Старый текст"), NewAuthor());

        comment.UpdateText(new CommentText("Новый текст"));

        Assert.Equal("Новый текст", comment.Text.Value);
    }

    [Fact]
    public void Comment_WithNullArticle_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new Comment(Guid.NewGuid(), null!, NewAuthor(), new CommentText("Текст")));
    }
}
