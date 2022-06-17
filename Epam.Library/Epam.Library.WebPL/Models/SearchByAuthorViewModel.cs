using System.ComponentModel.DataAnnotations;
using Epam.Library.Entities;

namespace Epam.Library.WebPL.Models;

public class SearchByAuthorViewModel
{
    [Required]
    public string Firstname { get; set; }
    
    [Required]
    public string Lastname { get; set; }
    
    [Required]
    public PolygraphyEnum.PolyType Type { get; set; }
}