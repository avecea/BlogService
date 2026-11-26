namespace BlogService.Application.Services;

using AutoMapper;
using BlogService.Application.Models.Author;
using BlogService.Application.Services.Abstractions;
using BlogService.Domain.Entities;
using BlogService.Domain.Repositories.Abstractions;

public class AuthorsApplicationService(IAuthorRepository repository, IMapper mapper)
    : IAuthorsApplicationService
{
    public async Task<IEnumerable<AuthorModel>> GetModelsAsync(CancellationToken cancellationToken = default)
        => (await repository.GetAllAsync(cancellationToken)).Select(mapper.Map<AuthorModel>);

    public async Task<AuthorModel?> GetModelByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var author = await repository.GetByIdAsync(id, cancellationToken);
        return author is null ? null : mapper.Map<AuthorModel>(author);
    }

    public async Task<AuthorModel?> GetAuthorByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var author = await repository.GetByEmailAsync(email, cancellationToken);
        return author is null ? null : mapper.Map<AuthorModel>(author);
    }

    public async Task<AuthorModel?> CreateModelAsync(CreateAuthorModel model, CancellationToken cancellationToken = default)
    {
        if (await repository.GetByEmailAsync(model.Email, cancellationToken) is not null)
            return null;

        var author = new Author(Guid.NewGuid(), model.Name, model.Email, model.Role);
        var createdAuthor = await repository.AddAsync(author, cancellationToken);
        return mapper.Map<AuthorModel>(createdAuthor);
    }

    public async Task<bool> UpdateModelAsync(AuthorModel model, CancellationToken cancellationToken = default)
    {
        var author = await repository.GetByIdAsync(model.Id, cancellationToken);
        if (author is null)
            return false;

        author.Update(model.Name, model.Email);
        await repository.UpdateAsync(author, cancellationToken);
        return true;
    }

    public async Task<bool> DeleteModelAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var author = await repository.GetByIdAsync(id, cancellationToken);
        if (author is null)
            return false;

        await repository.DeleteAsync(author, cancellationToken);
        return true;
    }

    public async Task<bool> PromoteToAuthorAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var author = await repository.GetByIdAsync(id, cancellationToken);
        if (author is null)
            return false;

        author.PromoteToAuthor();
        await repository.UpdateAsync(author, cancellationToken);
        return true;
    }
}
