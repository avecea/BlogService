namespace BlogService.ValueObjects;

using BlogService.ValueObjects.Base;
using BlogService.ValueObjects.Validators;

public class CommentText : ValueObject<string>
{
    public CommentText(string value) : base(value, new CommentTextValidator()) { }
}