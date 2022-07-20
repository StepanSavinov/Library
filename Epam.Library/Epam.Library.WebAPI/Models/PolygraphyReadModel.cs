namespace Epam.Library.WebAPI.Models;

public class PolygraphyReadModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime? Created { get; set; }
    public int TotalPages { get; set; }
    public string Footnote { get; set; }
}