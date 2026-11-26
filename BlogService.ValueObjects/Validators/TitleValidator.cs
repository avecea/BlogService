namespace BlogService.ValueObjects.Validators;

using BlogService.ValueObjects.Base;

public class TitleValidator : IValidator<string>
{
    public void Validate(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Заголовок не может быть пустым");
        if (value.Length > 200)
            throw new ArgumentException("Заголовок слишком длинный (макс. 200 символов)");
    }
}