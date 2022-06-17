using Epam.Library.BLL.Interfaces;
using Epam.Library.DAL.Interfaces;
using Epam.Library.Entities;

namespace Epam.Library.BLL;

public class LibraryLogic : ILibraryLogic
{
    private readonly ILibraryDao _libraryDao;
    private readonly IValidatable<Book> _bookValidator;
    private readonly IValidatable<Patent> _patentValidator;
    private readonly IValidatable<NewspaperIssue> _newspaperIssueValidator;
    private readonly IValidatable<Newspaper> _newspaperValidator;

    public LibraryLogic
    (
        ILibraryDao libraryDao,
        IValidatable<Book> bookValidator,
        IValidatable<Patent> patentValidator,
        IValidatable<NewspaperIssue> newspaperIssueValidator,
        IValidatable<Newspaper> newspaperValidator
    )
    {
        _libraryDao = libraryDao;
        _bookValidator = bookValidator;
        _patentValidator = patentValidator;
        _newspaperIssueValidator = newspaperIssueValidator;
        _newspaperValidator = newspaperValidator;
    }

    public bool AddToLibrary(Polygraphy polygraphy, out List<Error> errors)
    {
        errors = new List<Error>();

        switch (polygraphy)
        {
            case null:
                throw new ArgumentNullException(nameof(polygraphy));
            case Book book when !_bookValidator.IsValid(book, out errors):
                return false;
            case Book when _libraryDao.AddToLibrary(polygraphy):
                return true;
            case Book:
                errors.Add(new Error(ErrorType.Value, ErrorMessages.ErrorMessagePolygraphyAlreadyExist));
                return false;
            case Patent patent when !_patentValidator.IsValid(patent, out errors):
                return false;
            case Patent when _libraryDao.AddToLibrary(polygraphy):
                return true;
            case Patent:
                errors.Add(new Error(ErrorType.Value, ErrorMessages.ErrorMessagePolygraphyAlreadyExist));
                return false;
            case NewspaperIssue newspaperIssue when !_newspaperIssueValidator.IsValid(newspaperIssue, out errors):
                return false;
            case NewspaperIssue when _libraryDao.AddToLibrary(polygraphy):
                return true;
            case NewspaperIssue:
                errors.Add(new Error(ErrorType.Value, ErrorMessages.ErrorMessagePolygraphyAlreadyExist));
                return false;
            default:
                return false;
        }
    }

    public Dictionary<int, List<Polygraphy>> GroupByYear()
    {
        return _libraryDao.GetAllLibrary()
            .GroupBy(poly => poly.Created.Value.Year)
            .ToDictionary(x => x.Key, x => x.ToList());
    }

    public bool RemoveFromLibrary(int id, out List<Error> errors)
    {
        errors = new List<Error>();
        var poly = GetPolygraphyById(id);
        if (poly is null)
        {
            errors.Add(new Error(ErrorType.Value, ErrorMessages.ErrorMessagePolygraphyNotExist));
            return false;
        }

        if (poly.Id != id) return false;
        _libraryDao.RemoveFromLibrary(id);
        return true;
    }

    public bool MarkPolygraphyAsDeleted(int id)
    {
        var poly = GetPolygraphyById(id);
        if (poly is null)
        {
            return false;
        }

        if (poly.Id != id) return false;
        _libraryDao.MarkPolygraphyAsDeleted(id);
        return true;
    }

    public Polygraphy GetPolygraphyById(int id)
    {
        return _libraryDao.GetPolygraphyById(id);
    }

    public List<Polygraphy> SearchByName(string name)
    {
        return _libraryDao.SearchByName(name);
    }

    public List<Polygraphy> SearchByAuthor(string firstname, string lastname, PolygraphyEnum.PolyType type)
    {
        return _libraryDao.SearchByAuthor(firstname, lastname, type);
    }

    public Dictionary<string, List<Book>> GetBooksByPublisher(string publisher)
    {
        return _libraryDao.GetAllBooks()
            .Where(poly => poly.Publisher.StartsWith(publisher))
            .GroupBy(poly => poly.Publisher)
            .ToDictionary(x => x.Key, x => x.ToList());
    }

    public List<Polygraphy> GetAllLibrary()
    {
        return _libraryDao.GetAllLibrary();
    }

    public List<NewspaperIssue> GetAllNewspaperIssuesByNewspaperId(int id)
    {
        return _libraryDao.GetAllNewspaperIssuesByNewspaperId(id);
    }

    public List<Polygraphy> GetSortedPolygraphies(bool reverse)
    {
        return _libraryDao.GetSortedPolygraphies(reverse);
    }

    public void ClearLibrary()
    {
        _libraryDao.ClearLibrary();
    }

    public bool UpdatePolygraphyInLibrary(Polygraphy polygraphy, out List<Error> errors)
    {
        if (polygraphy is null)
        {
            throw new ArgumentNullException(nameof(polygraphy));
        }

        errors = new List<Error>();
        switch (polygraphy)
        {
            case Book book when !_bookValidator.IsValid(book, out errors):
                return false;
            case Book when _libraryDao.UpdatePolygraphyInLibrary(polygraphy):
                return true;
            case Book:
                errors.Add(new Error(ErrorType.Value, ErrorMessages.ErrorMessagePolygraphyAlreadyExist));
                return false;
            case NewspaperIssue newspaperIssue when !_newspaperIssueValidator.IsValid(newspaperIssue, out errors):
                return false;
            case NewspaperIssue when _libraryDao.UpdatePolygraphyInLibrary(polygraphy):
                return true;
            case NewspaperIssue:
                errors.Add(new Error(ErrorType.Value, ErrorMessages.ErrorMessagePolygraphyAlreadyExist));
                return false;
            case Patent patent when !_patentValidator.IsValid(patent, out errors):
                return false;
            case Patent when _libraryDao.UpdatePolygraphyInLibrary(polygraphy):
                return true;
            case Patent:
                errors.Add(new Error(ErrorType.Value, ErrorMessages.ErrorMessagePolygraphyAlreadyExist));
                return false;
            default:
                return false;
        }
    }

    // newspaper logic
    public bool AddNewspaperToLibrary(Newspaper newspaper, out List<Error> errors)
    {
        if (!_newspaperValidator.IsValid(newspaper, out errors)) return false;
        if (_libraryDao.AddNewspaperToLibrary(newspaper))
        {
            return true;
        }

        errors.Add(new Error(ErrorType.Value, ErrorMessages.ErrorMessageNewspaperAlreadyExist));
        return false;

    }

    public bool UpdateNewspaperInLibrary(Newspaper newspaper, out List<Error> errors)
    {
        if (!_newspaperValidator.IsValid(newspaper, out errors)) return false;
        if (_libraryDao.UpdateNewspaperInLibrary(newspaper))
        {
            return true;
        }

        errors.Add(new Error(ErrorType.Value, ErrorMessages.ErrorMessageNewspaperAlreadyExist));
        return false;

    }

    public bool RemoveNewspaperFromLibrary(int id, out List<Error> errors)
    {
        errors = new List<Error>();
        var newspaper = GetNewspaperById(id);
        if (newspaper is null)
        {
            errors.Add(new Error(ErrorType.Value, ErrorMessages.ErrorMessageNewspaperNotExist));
            return false;
        }

        if (newspaper.Id != id) return false;
        _libraryDao.RemoveNewspaperFromLibrary(id);
        return true;

    }

    public Newspaper GetNewspaperById(int id)
    {
        return _libraryDao.GetNewspaperById(id);
    }

    public List<Newspaper> GetAllNewspapers()
    {
        return _libraryDao.GetAllNewspapers();
    }

    public List<Newspaper> SearchNewspaperByName(string name)
    {
        return _libraryDao.SearchNewspaperByName(name);
    }

    public void ClearNewspapers()
    {
        _libraryDao.ClearNewspapers();
    }
}