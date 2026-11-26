namespace BlogService.ValueObjects;

using BlogService.ValueObjects.Base;
using BlogService.ValueObjects.Validators;

public class Content : ValueObject<string>
{
    public Content(string value) : base(value, new ContentValidator()) { }
}