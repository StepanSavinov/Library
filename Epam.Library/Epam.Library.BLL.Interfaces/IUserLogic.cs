using Epam.Library.Entities;

namespace Epam.Library.BLL.Interfaces;

public interface IUserLogic
{
    bool Auth(User user, out List<Error> errors);
    bool Register(User user, out List<Error> errors);
    bool RemoveUser(int id, out List<Error> errors);
    bool UpdateUser(User user, out List<Error> errors);
    List<User> GetAllUsers();
    User GetUserByUsername(string username);
    User GetUserById(int id);
    void ClearUsers();

}