namespace BlogService.WebHost.Controllers;

using BlogService.Application.Models.Article;
using BlogService.Application.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ArticlesController : ControllerBase
{
    private readonly IArticlesApplicationService _articles;

    public ArticlesController(IArticlesApplicationService articles)
    {
        _articles = articles;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ArticleModel>>> GetAll(CancellationToken ct)
    {
        return Ok(await _articles.GetModelsAsync(ct));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ArticleModel>> GetById(Guid id, CancellationToken ct)
    {
        var article = await _articles.GetModelByIdAsync(id, ct);
        if (article == null)
            return NotFound();

        return Ok(article);
    }

    [HttpGet("by-author/{authorId}")]
    public async Task<ActionResult<IEnumerable<ArticleModel>>> GetByAuthor(Guid authorId, CancellationToken ct)
    {
        return Ok(await _articles.GetArticlesByAuthorIdAsync(authorId, ct));
    }

    [HttpGet("published")]
    public async Task<ActionResult<IEnumerable<ArticleModel>>> GetPublished(CancellationToken ct)
    {
        return Ok(await _articles.GetPublishedArticlesAsync(ct));
    }

    [HttpPost]
    public async Task<ActionResult<ArticleModel>> Create([FromBody] CreateArticleModel model, CancellationToken ct)
    {
        var article = await _articles.CreateModelAsync(model, ct);
        if (article == null)
            return BadRequest("Author not found");

        return CreatedAtAction(nameof(GetById), new { id = article.Id }, article);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] ArticleModel model, CancellationToken ct)
    {
        if (id != model.Id)
            return BadRequest("Id mismatch");
        if (!await _articles.UpdateModelAsync(model, ct))
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        if (!await _articles.DeleteModelAsync(id, ct))
            return NotFound();

        return NoContent();
    }

    [HttpPost("{id}/publish")]
    public async Task<IActionResult> Publish(Guid id, CancellationToken ct)
    {
        if (!await _articles.PublishAsync(id, ct))
            return NotFound();

        return NoContent();
    }

    [HttpPost("{id}/archive")]
    public async Task<IActionResult> Archive(Guid id, CancellationToken ct)
    {
        if (!await _articles.ArchiveAsync(id, ct))
            return NotFound();

        return NoContent();
    }
}
