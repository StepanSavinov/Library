namespace Epam.Library.Entities;

public class Author : IEquatable<Author>
{
    public int Id { get; set; }
    public string Firstname { get; set; }
    public string Lastname { get; set; }

    public Author(string firstName, string lastName)
    {
        Firstname = firstName;
        Lastname = lastName;
    }
    public bool Equals(Author? other)
    {
        if (other is null)
        {
            return false;
        }

        return Firstname == other.Firstname && Lastname == other.Lastname;
    }

    public override bool Equals(object obj) => Equals(obj as Author);
    public override int GetHashCode() => (Firstname, Lastname).GetHashCode();
}