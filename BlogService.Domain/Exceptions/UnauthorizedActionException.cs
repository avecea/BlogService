namespace BlogService.Domain.Exceptions;

public sealed class UnauthorizedActionException(string message)
    : DomainException(message);