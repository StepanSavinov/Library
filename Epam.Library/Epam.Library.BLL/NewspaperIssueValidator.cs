using System.Text.RegularExpressions;
using Epam.Library.BLL.Interfaces;
using Epam.Library.Entities;

namespace Epam.Library.BLL;

public class NewspaperIssueValidator : PolygraphyValidator, IValidatable<NewspaperIssue>
{
    private const string CityRegex =
        @"^(?(?=[A-Z]+[a-z]+[ -]?[a-z]+[ -]?[A-Z]+[a-z])[A-Z]+[a-z]+[ -]?[a-z]+[ -]?[A-Z]+[a-z]+|[A-Z]+[a-z]+)|(?(?=[А-ЯЁ]+[а-яё]+[ -]?[а-яё]+[ -]?[А-ЯЁ]+[а-яё])[А-ЯЁ]+[а-яё]+[ -]?[а-яё]+[ -]?[А-ЯЁ]+[а-яё]+|[А-ЯЁ]+[а-яё]+)$";

    // private readonly ILibraryLogic _libraryLogic;
    // public NewspaperIssueValidator(ILibraryLogic libraryLogic) // circular dependency
    // {
    //     _libraryLogic = libraryLogic;
    // }
    
    public bool IsValid(NewspaperIssue issue, out List<Error> errors)
    {
        if (issue is null)
        {
            throw new ArgumentNullException();
        }

        base.IsValid(issue, out errors);
        //ValidateNewspaper(issue.NewspaperId, ref errors);
        ValidateCity(issue.City, ref errors);
        ValidatePublisher(issue.Publisher, ref errors);

        if (issue.Created != null)
        {
            ValidateCreationDate(issue.Created, ref errors);
        }

        if (issue.Number != null)
        {
            ValidateNumber(issue.Number, ref errors);
        }

        return !errors.Any();
    }

    // private void ValidateNewspaper(int issueNewspaperId, ref List<Error> errors)
    // {
    //     if (_libraryLogic.GetNewspaperById(issueNewspaperId) is null)
    //     {
    //         errors.Add(new Error(ErrorType.Value, ErrorMessages.ErrorMessageNewspaperNotExist));
    //     }
    // }

    private void ValidateCity(string city, ref List<Error> errors)
    {
        var cityPattern = new Regex(CityRegex);
        if (city.Length > 200)
            errors.Add(new Error(ErrorType.Length, ErrorMessages.ErrorMessagePolygraphyCityTooLong));

        else if (string.IsNullOrWhiteSpace(city))
            errors.Add(new Error(ErrorType.Empty, ErrorMessages.ErrorMessagePolygraphyCityEmpty));

        else if (!cityPattern.IsMatch(city))
            errors.Add(new Error(ErrorType.Format, ErrorMessages.ErrorMessagePolygraphyCityIncorrect));
    }

    private void ValidatePublisher(string publisher, ref List<Error> errors)
    {
        if (string.IsNullOrWhiteSpace(publisher))
            errors.Add(new Error(ErrorType.Empty, ErrorMessages.ErrorMessagePolygraphyPublisherEmpty));

        else if (publisher.Length > 300)
            errors.Add(new Error(ErrorType.Length, ErrorMessages.ErrorMessagePolygraphyPublisherTooLong));
    }

    private void ValidateNumber(int? number, ref List<Error> errors)
    {
        if (number <= 0)
            errors.Add(new Error(ErrorType.Value, ErrorMessages.ErrorMessagePolygraphyNumberNegative));
    }

    private void ValidateCreationDate(DateTime? date, ref List<Error> errors)
    {
        if (date!.Value.Year < 1474)
            errors.Add(new Error(ErrorType.Value, ErrorMessages.ErrorMessagePatentNewspaperDateTooEarly));

        else if (date.Value.Year > DateTime.Now.Year)
            errors.Add(new Error(ErrorType.Value, ErrorMessages.ErrorMessagePolygraphyDateFuture));
    }
}