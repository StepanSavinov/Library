using System.Text.RegularExpressions;
using Epam.Library.BLL.Interfaces;
using Epam.Library.Entities;

namespace Epam.Library.BLL;

public class BookValidator : PolygraphyValidator, IValidatable<Book>
{
    const string CityRegex =
        @"^(?(?=[A-Z]+[a-z]+[ -]?[a-z]+[ -]?[A-Z]+[a-z])[A-Z]+[a-z]+[ -]?[a-z]+[ -]?[A-Z]+[a-z]+|[A-Z]+[a-z]+)|(?(?=[А-ЯЁ]+[а-яё]+[ -]?[а-яё]+[ -]?[А-ЯЁ]+[а-яё])[А-ЯЁ]+[а-яё]+[ -]?[а-яё]+[ -]?[А-ЯЁ]+[а-яё]+|[А-ЯЁ]+[а-яё]+)$";

    const string IsbnRegex =
        @"^(?:ISBN(?:-10)?:? )?(?=[0-9X]{10}$|(?=(?:[0-9]+[- ]){3})[- 0-9X]{13}$)[0-9]{1,5}[- ]?[0-9]+[- ]?[0-9]+[- ]?[0-9X]$";

    private readonly IAuthorLogic _authorLogic;

    public BookValidator(IAuthorLogic authorLogic)
    {
        _authorLogic = authorLogic;
    }

    public bool IsValid(Book book, out List<Error> errors)
    {
        if (book is null)
        {
            throw new ArgumentNullException();
        }

        base.IsValid(book, out errors);
        ValidateAuthors(book.Authors, ref errors);
        ValidateCity(book.City, ref errors);
        ValidatePublisher(book.Publisher, ref errors);
        ValidateCreationDate(book.Created, ref errors);
        ValidateIsbn(book.ISBN, ref errors);

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
                     .Intersect(authorIds).Count() != authorIds.Count)
        {
            errors.Add(new Error(ErrorType.Value, ErrorMessages.ErrorMessagePolygraphyAuthorsNotExist));
        }
    }

    private void ValidateCity(string city, ref List<Error> errors)
    {
        Regex cityPattern = new Regex(CityRegex);
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

    private void ValidateCreationDate(DateTime? date, ref List<Error> errors)
    {
        if (date!.Value.Year < 1400)
            errors.Add(new Error(ErrorType.Value, ErrorMessages.ErrorMessagePolygraphyDateTooEarly));
        else if (date.Value.Year > DateTime.Now.Year)
            errors.Add(new Error(ErrorType.Value, ErrorMessages.ErrorMessagePolygraphyDateFuture));
    }

    private void ValidateIsbn(string isbn, ref List<Error> errors)
    {
        var isbnPattern = new Regex(IsbnRegex);
        if (!isbnPattern.IsMatch(isbn))
            errors.Add(new Error(ErrorType.Format, ErrorMessages.ErrorMessagePolygraphyIsbnIncorrect));
    }
}