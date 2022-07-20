namespace Epam.Library.WebAPI.Models;

public class UserLogin
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string Role { get; set; } = "User";
}