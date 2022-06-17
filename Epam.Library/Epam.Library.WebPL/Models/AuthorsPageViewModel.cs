using Epam.Library.Entities;

namespace Epam.Library.WebPL.Models;

public class AuthorsPageViewModel
{
    public IEnumerable<Author> Authors { get; set; }
    public PageViewModel PageViewModel { get; set; }
}