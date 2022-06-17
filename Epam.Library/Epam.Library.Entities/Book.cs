namespace Epam.Library.Entities;

public class Book : Polygraphy, IEquatable<Book>
{
    public List<int> Authors { get; set; }
    public string City { get; set; }
    public string Publisher { get; set; }
    public string ISBN { get; set; }

    public Book
    (
        string name, List<int> authors, string city, string publisher, 
        DateTime created, int totalPages, string footnote, string isbn
    )
    {
        Name = name;
        Authors = authors;
        City = city;
        Publisher = publisher;
        Created = created;
        TotalPages = totalPages;
        Footnote = footnote;
        ISBN = isbn;
    }

    public bool Equals(Book other)
    {
        if (other is null)
        {
            return false;
        }

        if (this.ISBN == other.ISBN)
        {
            return true;
        }
        else if(this.Name == other.Name && 
                Enumerable.SequenceEqual(this.Authors.OrderBy(x => x), other.Authors.OrderBy(x => x)) &&
                this.Created == other.Created)
        {
            return true;
        }
        return false;
    }

    public override bool Equals(object obj) => Equals(obj as Book);
    public override int GetHashCode() => (ISBN, Name, Authors, Created).GetHashCode();

}