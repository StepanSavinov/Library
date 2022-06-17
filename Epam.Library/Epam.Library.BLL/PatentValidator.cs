using System.Text.RegularExpressions;
using Epam.Library.BLL.Interfaces;
using Epam.Library.Entities;

namespace Epam.Library.BLL;

public class PatentValidator : PolygraphyValidator, IValidatable<Patent>
{
    private const string CountryRegex = @"^(?(?=[A-Z]+[a-z])[A-Z]+[a-z]+|[A-Z]+)|(?(?=[А-ЯЁ]+[а-яё])[А-ЯЁ]+[а-яё]+|[А-ЯЁ]+)$";
    private readonly IAuthorLogic _authorLogic;

    public PatentValidator(IAuthorLogic authorLogic)
    {
        _authorLogic = authorLogic;
    }

    public bool IsValid(Patent patent, out List<Error> errors)
    {
        if (patent is null)
        {
            throw new ArgumentNullException();
        }

        base.IsValid(patent, out errors);
        ValidateAuthors(patent.Authors, ref errors);
        ValidateCountry(patent.Country, ref errors);
        ValidateNumber(patent.Number, ref errors);
        ValidatePublicationDate(patent.Created, patent.Published, ref errors);
        ValidateCreationDate(patent.Created, ref errors);

        return !errors.Any();
    }

    private void ValidateAuthors(List<int> authorIds, ref List<Error> errors)
    {
        if (!authorIds.Any())
        {
            errors.Add(new Error(ErrorType.Empty, ErrorMessages.ErrorMessagePolygraphyAuthorsEmpty));
        }
        else if (_authorLogic.GetAuthorsByIds(authorIds)
                     .Select(author => author.Id)
                     .Intersect(authorIds).Count() != authorIds.Count())
        {
            errors.Add(new Error(ErrorType.Value, ErrorMessages.ErrorMessagePolygraphyAuthorsNotExist));
        }
    }

    private void ValidateCountry(string country, ref List<Error> errors)
    {
        var countryPattern = new Regex(CountryRegex);
        if (string.IsNullOrWhiteSpace(country))
            errors.Add(new Error(ErrorType.Empty, ErrorMessages.ErrorMessagePolygraphyCountryEmpty));

        else if (country.Length > 200)
            errors.Add(new Error(ErrorType.Length, ErrorMessages.ErrorMessagePolygraphyCountryTooLong));

        else if (!countryPattern.IsMatch(country))
            errors.Add(new Error(ErrorType.Format, ErrorMessages.ErrorMessagePolygraphyCountryIncorrect));
    }

    private void ValidateNumber(int number, ref List<Error> errors)
    {
        if (number > 999999999)
            errors.Add(new Error(ErrorType.Length, ErrorMessages.ErrorMessagePolygraphyNumberTooLong));
        else if (number <= 0)
            errors.Add(new Error(ErrorType.Value, ErrorMessages.ErrorMessagePolygraphyNumberNegative));
    }

    private void ValidateCreationDate(DateTime? date, ref List<Error> errors)
    {
        if (date is {Year: < 1474})
            errors.Add(new Error(ErrorType.Value, ErrorMessages.ErrorMessagePatentNewspaperDateTooEarly));

        else if (date != null && date.Value.Year > DateTime.Now.Year)
            errors.Add(new Error(ErrorType.Value, ErrorMessages.ErrorMessagePolygraphyDateFuture));
    }

    private void ValidatePublicationDate(DateTime? creationDate, DateTime publicationDate, ref List<Error> errors)
    {
        if (publicationDate.Year < 1474)
        {
            errors.Add(new Error(ErrorType.Value,
                ErrorMessages.ErrorMessagePatentNewspaperDateTooEarly));
        }
        else if (publicationDate.Year > DateTime.Now.Year)
        {
            errors.Add(new Error(ErrorType.Value,
                ErrorMessages.ErrorMessagePolygraphyPublicationDateFuture));
        }
        else if (creationDate != null && publicationDate < creationDate)
            errors.Add(new Error(ErrorType.Value,
                ErrorMessages.ErrorMessagePolygraphyPublicationLessThanCreation));
    }
}