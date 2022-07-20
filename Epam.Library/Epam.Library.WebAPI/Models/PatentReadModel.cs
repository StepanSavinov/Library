using Epam.Library.Entities;

namespace Epam.Library.WebAPI.Models;

public class PatentReadModel : PolygraphyReadModel
{
    public List<Author> Authors { get; set; }
    public int Number { get; set; }
    public DateTime Published { get; set; }
    public string Country { get; set; }
}