namespace BlogService.Application.Models.Author;

using BlogService.Application.Models.Base;
using BlogService.Domain.Enums;

public record class CreateAuthorModel(
    string Name,
    string Email,
    Role Role = Role.Reader) : ICreateModel;
