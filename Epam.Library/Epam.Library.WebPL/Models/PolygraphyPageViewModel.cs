using Epam.Library.Entities;

namespace Epam.Library.WebPL.Models;

public class PolygraphyPageViewModel
{
    public IEnumerable<PolygraphyReadViewModel> Polygraphies { get; set; }
    public IEnumerable<NewspaperIssueReadViewModel> Issues { get; set; }
    public IEnumerable<KeyValuePair<int, List<Polygraphy>>> GroupedByYear { get; set; }
    public IEnumerable<KeyValuePair<string, List<Book>>> BooksByPublisher { get; set; }
    public PageViewModel PageViewModel { get; set; }
}