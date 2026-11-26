namespace BlogService.Application.Services;

using AutoMapper;
using BlogService.Application.Models.Comment;
using BlogService.Application.Services.Abstractions;
using BlogService.Domain.Repositories.Abstractions;
using BlogService.ValueObjects;

public class CommentsApplicationService(
    ICommentRepository commentRepository,
    IArticleRepository articleRepository,
    IAuthorRepository authorRepository,
    IMapper mapper) : ICommentsApplicationService
{
    public async Task<IEnumerable<CommentModel>> GetModelsAsync(CancellationToken cancellationToken = default)
        => (await commentRepository.GetAllAsync(cancellationToken)).Select(mapper.Map<CommentModel>);

    public async Task<CommentModel?> GetModelByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var comment = await commentRepository.GetByIdAsync(id, cancellationToken);
        return comment is null ? null : mapper.Map<CommentModel>(comment);
    }

    public async Task<IEnumerable<CommentModel>> GetCommentsByArticleIdAsync(Guid articleId, CancellationToken cancellationToken = default)
        => (await commentRepository.GetByArticleIdAsync(articleId, cancellationToken)).Select(mapper.Map<CommentModel>);

    public async Task<IEnumerable<CommentModel>> GetVisibleCommentsByArticleIdAsync(Guid articleId, CancellationToken cancellationToken = default)
        => (await commentRepository.GetVisibleByArticleIdAsync(articleId, cancellationToken)).Select(mapper.Map<CommentModel>);

    public async Task<CommentModel?> CreateModelAsync(CreateCommentModel model, CancellationToken cancellationToken = default)
    {
        var article = await articleRepository.GetByIdAsync(model.ArticleId, cancellationToken);
        if (article is null)
            return null;

        var author = await authorRepository.GetByIdAsync(model.AuthorId, cancellationToken);
        if (author is null)
            return null;

        var comment = article.AddComment(new CommentText(model.Text), author);
        var createdComment = await commentRepository.AddAsync(comment, cancellationToken);
        return mapper.Map<CommentModel>(createdComment);
    }

    public async Task<bool> UpdateModelAsync(CommentModel model, CancellationToken cancellationToken = default)
    {
        var comment = await commentRepository.GetByIdAsync(model.Id, cancellationToken);
        if (comment is null)
            return false;

        comment.UpdateText(new CommentText(model.Text));
        await commentRepository.UpdateAsync(comment, cancellationToken);
        return true;
    }

    public async Task<bool> DeleteModelAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var comment = await commentRepository.GetByIdAsync(id, cancellationToken);
        if (comment is null)
            return false;

        await commentRepository.DeleteAsync(comment, cancellationToken);
        return true;
    }

    public Task<bool> HideCommentAsync(Guid commentId, Guid requesterAuthorId, CancellationToken cancellationToken = default)
        => ChangeVisibilityAsync(commentId, requesterAuthorId, hide: true, cancellationToken);

    public Task<bool> ShowCommentAsync(Guid commentId, Guid requesterAuthorId, CancellationToken cancellationToken = default)
        => ChangeVisibilityAsync(commentId, requesterAuthorId, hide: false, cancellationToken);

    private async Task<bool> ChangeVisibilityAsync(
        Guid commentId,
        Guid requesterAuthorId,
        bool hide,
        CancellationToken cancellationToken)
    {
        var comment = await commentRepository.GetByIdAsync(commentId, cancellationToken);
        if (comment is null)
            return false;

        var article = await articleRepository.GetByIdAsync(comment.ArticleId, cancellationToken);
        var requester = await authorRepository.GetByIdAsync(requesterAuthorId, cancellationToken);
        if (article is null || requester is null)
            return false;

        if (hide)
            article.HideComment(comment, requester);
        else
            article.ShowComment(comment, requester);

        await commentRepository.UpdateAsync(comment, cancellationToken);
        return true;
    }
}
