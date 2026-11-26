namespace BlogService.ValueObjects.Validators;

using BlogService.ValueObjects.Base;

public class ContentValidator : IValidator<string>
{
    public void Validate(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Текст статьи не может быть пустым");
        if (value.Length > 50000)
            throw new ArgumentException("Текст слишком длинный");
    }
}