using Epam.Library.Entities;

namespace Epam.Library.WebAPI.Models;

public class BookReadModel : PolygraphyReadModel
{
    public List<Author> Authors { get; set; }
    public string City { get; set; }
    public string Publisher { get; set; }
    public string ISBN { get; set; }
}