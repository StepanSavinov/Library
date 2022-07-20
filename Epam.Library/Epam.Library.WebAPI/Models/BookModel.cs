namespace Epam.Library.WebAPI.Models;

public class BookModel
{
    public string Name { get; set; }
    
    public DateTime Created { get; set; }
    
    public int TotalPages { get; set; }
    
    public string Footnote { get; set; }
    
    public List<int> Authors { get; set; }
    
    public string City { get; set; }
    
    public string Publisher { get; set; }
    
    public string ISBN { get; set; }
}