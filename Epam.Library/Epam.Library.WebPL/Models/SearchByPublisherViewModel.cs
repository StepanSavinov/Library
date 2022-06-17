using System.ComponentModel.DataAnnotations;

namespace Epam.Library.WebPL.Models;

public class SearchByPublisherViewModel
{
    [Required] 
    public string Publisher { get; set; }
}