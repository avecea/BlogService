namespace BlogService.Application.Models.Author;

using BlogService.Application.Models.Base;
using BlogService.Domain.Enums;

public record class AuthorModel(
    Guid Id,
    string Name,
    string Email,
    Role Role,
    DateTime CreatedAt) : IModel<Guid>;
