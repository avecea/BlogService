namespace BlogService.Domain.Exceptions;

public sealed class ArticleNotFoundException(Guid id)
    : DomainException($"Статья с идентификатором {id} не найдена.");