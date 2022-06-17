namespace Epam.Library.Entities;

public class Error : IEquatable<Error>
{
    public int Id { get; set; }
    public ErrorType Type { get; set; }
    public string Message { get; set; }

    public Error(ErrorType type, string message)
    {
        Type = type;
        Message = message;
    }

    public bool Equals(Error? other)
    {
        if (other is null)
        {
            return false;
        }

        return Type == other.Type && Message == other.Message;
    }

    public override bool Equals(object obj) => Equals(obj as Error);
    public override int GetHashCode() => (Type, Message).GetHashCode();
}
public enum ErrorType
{
    Format,
    Length,
    Empty,
    Value
}