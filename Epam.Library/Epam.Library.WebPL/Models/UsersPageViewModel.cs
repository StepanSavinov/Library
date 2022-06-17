using Epam.Library.Entities;

namespace Epam.Library.WebPL.Models;

public class UsersPageViewModel
{
    public IEnumerable<User> Users { get; set; }
    public PageViewModel PageViewModel { get; set; }
}