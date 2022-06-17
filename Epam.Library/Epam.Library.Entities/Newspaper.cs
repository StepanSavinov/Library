namespace Epam.Library.Entities;

public class Newspaper : IEquatable<Newspaper>
{
    public int Id { get; set; }
    public List<int> Issues { get; set; }
    public string Name { get; set; }
    public string ISSN { get; set; }

    public Newspaper(string name, List<int> issues, string issn)
    {
        Name = name;
        Issues = issues;
        ISSN = issn;
    }
    public bool Equals(Newspaper? other)
    {
        if (other is null)
        {
            return false;
        }

        return this.ISSN == other.ISSN && this.Name == other.Name;
    }

    public override bool Equals(object obj) => Equals(obj as Newspaper);
    public override int GetHashCode() => (ISSN, Name).GetHashCode();
}