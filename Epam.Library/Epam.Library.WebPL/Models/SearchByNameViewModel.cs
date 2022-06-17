using System.ComponentModel.DataAnnotations;

namespace Epam.Library.WebPL.Models;

public class SearchByNameViewModel
{
    [Required] 
    public string Name { get; set; }
}