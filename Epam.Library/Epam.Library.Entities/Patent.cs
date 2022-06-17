namespace Epam.Library.Entities;

public class Patent : Polygraphy, IEquatable<Patent>
{
    public List<int> Authors { get; set; }
    public string Country { get; set; }
    public int Number { get; set; }
    public DateTime Published { get; set; }

    public Patent
    (
        string name, List<int> authors, string country, int number,
        DateTime? created, DateTime published, int totalPages, string footnote
    )
    {
        Name = name;
        Authors = authors;
        Country = country;
        Number = number;
        Created = created;
        Published = published;
        TotalPages = totalPages;
        Footnote = footnote;
    }

    public bool Equals(Patent other)
    {
        if (other is null)
        {
            return false;
        }

        return this.Number == other.Number && Country == other.Country;
    }

    public override bool Equals(object obj) => Equals(obj as Patent);
    public override int GetHashCode() => (Number, Country).GetHashCode();

}