namespace Epam.Library.Entities;

public class NewspaperIssue : Polygraphy, IEquatable<NewspaperIssue>
{
    public int NewspaperId { get; set; }
    public string City { get; set; }
    public string Publisher { get; set; }
    public int? Number { get; set; }

    public NewspaperIssue
    (
        string name, int number, string city, string publisher, DateTime created, 
        int totalPages, string footnote
    )
    {
        Name = name;
        Number = number;
        City = city;
        Publisher = publisher;
        Created = created;
        TotalPages = totalPages;
        Footnote = footnote;
    }

    public bool Equals(NewspaperIssue other)
    {
        if (other is null)
        {
            return false;
        }

        return Name == other.Name && Publisher == other.Publisher && Created == other.Created;
    }

    public override bool Equals(object obj) => Equals(obj as NewspaperIssue);
    public override int GetHashCode() => (Name, Publisher, Created).GetHashCode();

}