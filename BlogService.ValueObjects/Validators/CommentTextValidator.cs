
namespace BlogService.ValueObjects.Validators;

using BlogService.ValueObjects.Base;

public class CommentTextValidator : IValidator<string>
{
    public void Validate(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Комментарий не может быть пустым");
        if (value.Length > 1000)
            throw new ArgumentException("Комментарий слишком длинный");
    }
}