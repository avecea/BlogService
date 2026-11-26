namespace BlogService.ValueObjects.Base;

public abstract class ValueObject<T>
{
    public T Value { get; private set; }

    protected ValueObject(T value, IValidator<T> validator)
    {
        validator.Validate(value);
        Value = value;
    }

    public override string ToString() => Value.ToString();

    public override bool Equals(object obj)
    {
        return obj is ValueObject<T> other && EqualityComparer<T>.Default.Equals(Value, other.Value);
    }

    public override int GetHashCode() => Value.GetHashCode();
}