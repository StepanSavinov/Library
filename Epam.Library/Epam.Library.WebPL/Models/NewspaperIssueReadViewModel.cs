namespace Epam.Library.WebPL.Models;

public class NewspaperIssueReadViewModel : PolygraphyReadViewModel
{
    public string City { get; set; }
    public string Publisher { get; set; }
    public int Number { get; set; }
    public string ISSN { get; set; }
    public int NewspaperId { get; set; }
}