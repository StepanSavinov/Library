using System.ComponentModel.DataAnnotations;

namespace Epam.Library.WebPL.Models;

public class NewspaperIssueViewModel
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
    public string City { get; set; }
    [Required]
    public string Publisher { get; set; }
    
    public int Number { get; set; }
    [Required]
    public int NewspaperId { get; set; }
}