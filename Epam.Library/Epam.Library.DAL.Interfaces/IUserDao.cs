using Epam.Library.Entities;

namespace Epam.Library.DAL.Interfaces;

public interface IUserDao
{
    bool Auth(User user);
    bool Register(User user);
    void RemoveUser(int id);
    bool UpdateUser(User user);
    User GetUserById(int id);
    List<User> GetAllUsers();
    User GetUserByUsername(string username);
    void ClearUsers();
}