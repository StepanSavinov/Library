using System.ComponentModel.DataAnnotations;

namespace Epam.Library.WebPL.Models;

public class AuthorViewModel
{
    [Required]
    public string Firstname { get; set; }
    
    [Required]
    public string Lastname { get; set; }
}