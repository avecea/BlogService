namespace BlogService.Application.Services.Abstractions;

using BlogService.Application.Models.Comment;
using BlogService.Application.Services.Abstractions.Base;

public interface ICommentsApplicationService : IApplicationService<CommentModel, CreateCommentModel, Guid>
{
    Task<IEnumerable<CommentModel>> GetCommentsByArticleIdAsync(Guid articleId, CancellationToken cancellationToken = default);

    Task<IEnumerable<CommentModel>> GetVisibleCommentsByArticleIdAsync(Guid articleId, CancellationToken cancellationToken = default);

    Task<bool> HideCommentAsync(Guid commentId, Guid requesterAuthorId, CancellationToken cancellationToken = default);

    Task<bool> ShowCommentAsync(Guid commentId, Guid requesterAuthorId, CancellationToken cancellationToken = default);
}
