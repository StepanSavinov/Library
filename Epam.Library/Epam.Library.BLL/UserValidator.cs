using System.Text.RegularExpressions;
using Epam.Library.BLL.Interfaces;
using Epam.Library.Entities;

namespace Epam.Library.BLL;

public class UserValidator : IValidatable<User>
{
    private const string UsernamePattern = @"^[A-Za-z0-9_]+$";
    public bool IsValid(User user, out List<Error> errors)
    {
        errors = new List<Error>();
        if (user is null)
            throw new ArgumentNullException();

        ValidateUsername(user.Username, ref errors);
        ValidatePassword(user.Password, user.Username, ref errors);

        return !errors.Any();
    }

    private void ValidateUsername(string username, ref List<Error> errors)
    {
        Regex usernameRegex = new Regex(UsernamePattern);
        if (username.StartsWith("_") || 
            username.EndsWith("_") || 
            char.IsDigit(username[0]) || 
            username.Contains("__") || 
            !usernameRegex.IsMatch(username))
        {
            errors.Add(new Error(ErrorType.Value, ErrorMessages.ErrorMessageUserIncorrectUsername));
        }
        else if (string.IsNullOrEmpty(username))
        {
            errors.Add(new Error(ErrorType.Empty, ErrorMessages.ErrorMessageUserEmptyUsername));
        }
    }

    private void ValidatePassword(string password, string username, ref List<Error> errors)
    {
        if (password.Length < 3)
        {
            errors.Add(new Error(ErrorType.Length, ErrorMessages.ErrorMessageUserPasswordShort));
        }
        else if (password.Contains(username))
        {
            errors.Add(new Error(ErrorType.Value, ErrorMessages.ErrorMessageUserPasswordContainsUsername));
        }
        else if (string.IsNullOrEmpty(password))
        {
            errors.Add(new Error(ErrorType.Empty, ErrorMessages.ErrorMessageUserEmptyPassword));
        }
    }
}