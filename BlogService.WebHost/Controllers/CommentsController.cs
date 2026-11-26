namespace BlogService.WebHost.Controllers;

using BlogService.Application.Models.Comment;
using BlogService.Application.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/articles/{articleId}/[controller]")]
public class CommentsController : ControllerBase
{
    private readonly ICommentsApplicationService _comments;

    public CommentsController(ICommentsApplicationService comments)
    {
        _comments = comments;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CommentModel>>> GetByArticle(Guid articleId, CancellationToken ct)
    {
        return Ok(await _comments.GetCommentsByArticleIdAsync(articleId, ct));
    }

    [HttpGet("visible")]
    public async Task<ActionResult<IEnumerable<CommentModel>>> GetVisibleByArticle(Guid articleId, CancellationToken ct)
    {
        return Ok(await _comments.GetVisibleCommentsByArticleIdAsync(articleId, ct));
    }

    [HttpGet("{commentId}")]
    public async Task<ActionResult<CommentModel>> GetById(Guid articleId, Guid commentId, CancellationToken ct)
    {
        var comment = await _comments.GetModelByIdAsync(commentId, ct);
        if (comment == null || comment.ArticleId != articleId)
            return NotFound();

        return Ok(comment);
    }

    [HttpPost]
    public async Task<ActionResult<CommentModel>> Create(Guid articleId, [FromBody] CreateCommentDto dto, CancellationToken ct)
    {
        var comment = await _comments.CreateModelAsync(
            new CreateCommentModel(articleId, dto.AuthorId, dto.Text), ct);

        if (comment == null)
            return BadRequest("Article or author not found");

        return CreatedAtAction(nameof(GetById), new { articleId, commentId = comment.Id }, comment);
    }

    [HttpPut("{commentId}")]
    public async Task<IActionResult> Update(Guid articleId, Guid commentId, [FromBody] CommentModel model, CancellationToken ct)
    {
        if (commentId != model.Id || articleId != model.ArticleId)
            return BadRequest("Id mismatch");
        if (!await _comments.UpdateModelAsync(model, ct))
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{commentId}")]
    public async Task<IActionResult> Delete(Guid articleId, Guid commentId, CancellationToken ct)
    {
        var comment = await _comments.GetModelByIdAsync(commentId, ct);
        if (comment == null || comment.ArticleId != articleId)
            return NotFound();
        if (!await _comments.DeleteModelAsync(commentId, ct))
            return NotFound();

        return NoContent();
    }

    [HttpPost("{commentId}/hide")]
    public async Task<IActionResult> HideComment(Guid articleId, Guid commentId, [FromBody] HideCommentDto dto, CancellationToken ct)
    {
        var comment = await _comments.GetModelByIdAsync(commentId, ct);
        if (comment == null || comment.ArticleId != articleId)
            return NotFound("Comment not found");
        if (!await _comments.HideCommentAsync(commentId, dto.AuthorId, ct))
            return BadRequest("Article or author not found");

        return NoContent();
    }

    [HttpPost("{commentId}/show")]
    public async Task<IActionResult> ShowComment(Guid articleId, Guid commentId, [FromBody] HideCommentDto dto, CancellationToken ct)
    {
        var comment = await _comments.GetModelByIdAsync(commentId, ct);
        if (comment == null || comment.ArticleId != articleId)
            return NotFound("Comment not found");
        if (!await _comments.ShowCommentAsync(commentId, dto.AuthorId, ct))
            return BadRequest("Article or author not found");

        return NoContent();
    }
}

public record CreateCommentDto(Guid AuthorId, string Text);
public record HideCommentDto(Guid AuthorId);
