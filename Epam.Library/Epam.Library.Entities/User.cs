namespace Epam.Library.Entities;

public class User : IEquatable<User>
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }

    public User(string username, string password)
    {
        Username = username;
        Password = password;
        Role = "User";
    }

    public User(string username, string password, string role)
    {
        Username = username;
        Password = password;
        Role = role;
    }

    public bool Equals(User? other)
    {
        if (other is null)
        {
            return false;
        }

        return this.Username == other.Username;
    }

    public override bool Equals(object obj) => Equals(obj as User);
    public override int GetHashCode() => (Username).GetHashCode();
}