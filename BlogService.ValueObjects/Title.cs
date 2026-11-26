namespace BlogService.ValueObjects;

using BlogService.ValueObjects.Base;
using BlogService.ValueObjects.Validators;

public class Title : ValueObject<string>
{
    public Title(string value) : base(value, new TitleValidator()) { }
}