using System.Text.RegularExpressions;
using Epam.Library.BLL.Interfaces;
using Epam.Library.Entities;

namespace Epam.Library.BLL;

public class AuthorValidator : IValidatable<Author>
{
    const string FirstnameRegex =
        @"^(?(?=[A-Z]?[a-z]+[-])[A-Z]?[a-z]+[-]?[A-Z]+[a-z]+|[A-Z]+[a-z]+)|(?(?=[А-ЯЁ]?[а-яё]+[-])[А-ЯЁ]?[а-яё]+[-]?[А-ЯЁ]+[а-яё]+|[А-ЯЁ]+[а-яё]+)$";

    const string LastnameRegex =
        @"^(?(?=[a-z]+[' -]+[A-Z]+[a-z]+)[a-z]+[' -]+[A-Z]+[a-z]+|(?(?=[а-яё]+[' -]+[А-ЯЁ]+[а-яё]+)[а-яё]+[' -]+[А-ЯЁ]+[а-яё]+)(?(?=[A-Z]+[a-z]+)[A-Z]+[a-z]+)(?(?=[А-ЯЁ]+[а-яё]+)[А-ЯЁ]+[а-яё]+))$";

    public bool IsValid(Author author, out List<Error> errors)
    {
        errors = new List<Error>();
        if (author is null)
            throw new ArgumentNullException();

        ValidateFirstname(author.Firstname, ref errors);
        ValidateLastname(author.Lastname, ref errors);

        return !errors.Any();
    }

    private void ValidateFirstname(string firstname, ref List<Error> errors)
    {
        Regex firstnamePattern = new Regex(FirstnameRegex);
        if (String.IsNullOrWhiteSpace(firstname))
        {
            errors.Add(new Error(ErrorType.Empty, ErrorMessages.ErrorMessageAuthorFirstnameEmpty));
        }
        else if (!firstnamePattern.IsMatch(firstname))
        {
            errors.Add(new Error(ErrorType.Format, ErrorMessages.ErrorMessageAuthorFirstnameIncorrect));
        }
        else if (firstname.Length > 50)
        {
            errors.Add(new Error(ErrorType.Length, ErrorMessages.ErrorMessageAuthorFirstnameTooLong));
        }
    }

    private void ValidateLastname(string lastname, ref List<Error> errors)
    {
        Regex lastnamePattern = new Regex(LastnameRegex);
        if (String.IsNullOrWhiteSpace(lastname))
        {
            errors.Add(new Error(ErrorType.Empty, ErrorMessages.ErrorMessageAuthorLastnameEmpty));
        }
        else if (!lastnamePattern.IsMatch(lastname))
        {
            errors.Add(new Error(ErrorType.Format, ErrorMessages.ErrorMessageAuthorLastnameIncorrect));
        }
        else if (lastname.Length > 200)
        {
            errors.Add(new Error(ErrorType.Length, ErrorMessages.ErrorMessageAuthorLastnameTooLong));
        }
    }
}