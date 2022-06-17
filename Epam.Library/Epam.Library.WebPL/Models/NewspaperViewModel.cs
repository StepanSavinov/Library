using System.ComponentModel.DataAnnotations;

namespace Epam.Library.WebPL.Models;

public class NewspaperViewModel
{
    [Required]
    public string Name { get; set; }
    
    [Required]
    public string ISSN { get; set; }
}