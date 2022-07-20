using Epam.Library.Entities;

namespace Epam.Library.WebAPI.Models;

public class SearchByAuthorModel
{
    public string Firstname { get; set; }
    
    public string Lastname { get; set; }
    
    public PolygraphyEnum.PolyType Type { get; set; }
}