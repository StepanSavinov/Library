using Epam.Library.BLL.Interfaces;
using Epam.Library.DAL.Interfaces;
using Epam.Library.Entities;

namespace Epam.Library.BLL;

public class UserLogic : IUserLogic
{
    private readonly IUserDao _userDao;
    private readonly IValidatable<User> _userValidator;

    public UserLogic(IUserDao userDao, IValidatable<User> userValidator)
    {
        _userDao = userDao;
        _userValidator = userValidator;
    }

    public bool Auth(User user, out List<Error> errors)
    {
        return _userValidator.IsValid(user, out errors) && _userDao.Auth(user);
    }

    public List<User> GetAllUsers()
    {
        return _userDao.GetAllUsers();
    }

    public User GetUserByUsername(string username)
    {
        return _userDao.GetUserByUsername(username);
    }

    public bool Register(User user, out List<Error> errors)
    {
        if (!_userValidator.IsValid(user, out errors)) return false;
        _userDao.Register(user);
        
        return true;
    }

    public User GetUserById(int id)
    {
        return _userDao.GetUserById(id);
    }

    public bool RemoveUser(int id, out List<Error> errors)
    {
        errors = new List<Error>();
        var user = GetUserById(id);
        if (user is null)
        {
            errors.Add(new Error(ErrorType.Value, ErrorMessages.ErrorMessageUserNotExist));
            return false;
        }
        if (user.Id == id)
        {
            _userDao.RemoveUser(id);
            return true;
        }

        return false;
    }

    public bool UpdateUser(User user, out List<Error> errors)
    {
        if (user is null)
        {
            throw new ArgumentNullException();
        }

        if (!_userValidator.IsValid(user, out errors)) return false;
        if (_userDao.UpdateUser(user))
        {
            return true;
        }
        
        errors.Add(new Error(ErrorType.Value, ErrorMessages.ErrorMessageUserAlreadyExist));
        return false;

    }

    public void ClearUsers()
    {
        _userDao.ClearUsers();
    }
}