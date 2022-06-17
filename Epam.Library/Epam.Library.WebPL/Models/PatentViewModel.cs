using System.ComponentModel.DataAnnotations;

namespace Epam.Library.WebPL.Models;

public class PatentViewModel
{
    [Required]
    public string Name { get; set; }
    
    [DataType(DataType.Date)]
    public DateTime? Created { get; set; }
    
    [Required]
    public int TotalPages { get; set; }
    
    [Required]
    public string Footnote { get; set; }
    
    [Required]
    public List<int> Authors { get; set; }

    [Required]
    public int Number { get; set; }
    
    [Required]
    [DataType(DataType.Date)]
    public DateTime Published { get; set; }
    
    [Required]
    public string Country { get; set; }
}