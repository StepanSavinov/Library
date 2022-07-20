namespace Epam.Library.WebAPI.Models;

public class NewspaperIssueModel
{
    public string Name { get; set; }
    
    public DateTime Created { get; set; }
    
    public int TotalPages { get; set; }
    
    public string Footnote { get; set; }
    
    public string City { get; set; }
    
    public string Publisher { get; set; }
    
    public int Number { get; set; }
    
    public int NewspaperId { get; set; }
}