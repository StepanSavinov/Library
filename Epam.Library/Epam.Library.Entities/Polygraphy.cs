namespace Epam.Library.Entities;

public abstract class Polygraphy
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime? Created { get; set; }
    public int TotalPages { get; set; }
    public string Footnote { get; set; }
    public Polygraphy ObjectType { get; private set; }

    protected Polygraphy()
    {
        ObjectType = this;
    }

}