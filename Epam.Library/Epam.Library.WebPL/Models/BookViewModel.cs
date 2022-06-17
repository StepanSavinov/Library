using System.ComponentModel.DataAnnotations;

namespace Epam.Library.WebPL.Models;

public class BookViewModel
{
    [Required]
    public string Name { get; set; }
    
    [Required]
    [DataType(DataType.Date)]
    public DateTime Created { get; set; }
    
    [Required]
    public int TotalPages { get; set; }
    
    [Required]
    public string Footnote { get; set; }
    
    [Required]
    public List<int> Authors { get; set; }
    
    [Required]
    public string City { get; set; }
    
    [Required]
    public string Publisher { get; set; }
    
    public string ISBN { get; set; }
}