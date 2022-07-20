namespace Epam.Library.WebAPI.Models;

public class NewspaperIssueReadModel : PolygraphyReadModel
{
    public string City { get; set; }
    public string Publisher { get; set; }
    public int Number { get; set; }
    public string ISSN { get; set; }
    public int NewspaperId { get; set; }
}