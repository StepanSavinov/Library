namespace Epam.Library.WebAPI.Models;

public class PatentModel
{
    public string Name { get; set; }
    
    public DateTime? Created { get; set; }
    
    public int TotalPages { get; set; }
    
    public string Footnote { get; set; }
    
    public List<int> Authors { get; set; }
    
    public int Number { get; set; }
    
    public DateTime Published { get; set; }
    
    public string Country { get; set; }
}