namespace BlogService.WebHost.Controllers;

using BlogService.Application.Models.Author;
using BlogService.Application.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthorsController : ControllerBase
{
    private readonly IAuthorsApplicationService _authors;

    public AuthorsController(IAuthorsApplicationService authors)
    {
        _authors = authors;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AuthorModel>>> GetAll(CancellationToken ct)
    {
        return Ok(await _authors.GetModelsAsync(ct));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AuthorModel>> GetById(Guid id, CancellationToken ct)
    {
        var author = await _authors.GetModelByIdAsync(id, ct);
        if (author == null)
            return NotFound();

        return Ok(author);
    }

    [HttpGet("by-email/{email}")]
    public async Task<ActionResult<AuthorModel>> GetByEmail(string email, CancellationToken ct)
    {
        var author = await _authors.GetAuthorByEmailAsync(email, ct);
        if (author == null)
            return NotFound();

        return Ok(author);
    }

    [HttpPost]
    public async Task<ActionResult<AuthorModel>> Create([FromBody] CreateAuthorModel model, CancellationToken ct)
    {
        var author = await _authors.CreateModelAsync(model, ct);
        if (author == null)
            return BadRequest("Author with this email already exists");

        return CreatedAtAction(nameof(GetById), new { id = author.Id }, author);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] AuthorModel model, CancellationToken ct)
    {
        if (id != model.Id)
            return BadRequest("Id mismatch");
        if (!await _authors.UpdateModelAsync(model, ct))
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        if (!await _authors.DeleteModelAsync(id, ct))
            return NotFound();

        return NoContent();
    }

    [HttpPost("{id}/promote")]
    public async Task<IActionResult> PromoteToAuthor(Guid id, CancellationToken ct)
    {
        if (!await _authors.PromoteToAuthorAsync(id, ct))
            return NotFound();

        return NoContent();
    }
}
