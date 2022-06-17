using Epam.Library.Entities;

namespace Epam.Library.WebPL.Models;

public class BookReadViewModel : PolygraphyReadViewModel
{
    public List<Author> Authors { get; set; }
    public string City { get; set; }
    public string Publisher { get; set; }
    public string ISBN { get; set; }
}