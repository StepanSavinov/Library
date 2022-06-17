using Epam.Library.Entities;

namespace Epam.Library.WebPL.Models;

public class PatentReadViewModel : PolygraphyReadViewModel
{
    public List<Author> Authors { get; set; }
    public int Number { get; set; }
    public DateTime Published { get; set; }
    public string Country { get; set; }
}