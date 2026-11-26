namespace BlogService.Domain.Exceptions;

public sealed class ValidationException(string field, string message)
    : DomainException($"Ошибка в поле [{field}]: {message}");