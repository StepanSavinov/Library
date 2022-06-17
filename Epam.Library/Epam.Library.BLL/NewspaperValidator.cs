using System.Text.RegularExpressions;
using Epam.Library.BLL.Interfaces;
using Epam.Library.Entities;

namespace Epam.Library.BLL;

public class NewspaperValidator : IValidatable<Newspaper>
{
    private const string IssnRegex = @"^ISSN-\d{4}-\d{4}$";

    public bool IsValid(Newspaper newspaper, out List<Error> errors)
    {
        if (newspaper is null)
        {
            throw new ArgumentNullException();
        }

        errors = new List<Error>();

        ValidateName(newspaper.Name, ref errors);
        ValidateIssn(newspaper.ISSN, ref errors);

        return !errors.Any();
    }

    private void ValidateName(string name, ref List<Error> errors)
    {
        if (String.IsNullOrWhiteSpace(name))
            errors.Add(new Error(ErrorType.Empty, ErrorMessages.ErrorMessagePolygraphyNameEmpty));
        else if (name.Length > 300)
            errors.Add(new Error(ErrorType.Length, ErrorMessages.ErrorMessagePolygraphyNameTooLong));
    }

    private void ValidateIssn(string issn, ref List<Error> errors)
    {
        Regex issnPattern = new Regex(IssnRegex);
        if (!string.IsNullOrWhiteSpace(issn))
        {
            if (!issnPattern.IsMatch(issn))
            {
                errors.Add(new Error(ErrorType.Format,
                    ErrorMessages.ErrorMessagePolygraphyIssnIncorrect));
            }
        }
    }
}