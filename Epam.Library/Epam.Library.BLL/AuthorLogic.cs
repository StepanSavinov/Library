using Epam.Library.BLL.Interfaces;
using Epam.Library.DAL.Interfaces;
using Epam.Library.Entities;

namespace Epam.Library.BLL;

public class AuthorLogic : IAuthorLogic
{
    private IAuthorDao _authorDao;
    private IValidatable<Author> _authorValidator;

    public AuthorLogic(IAuthorDao authorDao, IValidatable<Author> authorValidator)
    {
        _authorDao = authorDao;
        _authorValidator = authorValidator;
    }

    public bool AddAuthor(Author author, out List<Error> errors)
    {
        if (_authorValidator.IsValid(author, out errors))
        {
            if (_authorDao.AddAuthor(author))
                return true;
            else
                errors.Add(new Error(ErrorType.Value, ErrorMessages.ErrorMessageAuthorAlreadyExist));
            return false;
        }

        return false;
    }

    public bool UpdateAuthor(Author author, out List<Error> errors)
    {
        if (author is null)
        {
            throw new ArgumentNullException();
        }

        if (!_authorValidator.IsValid(author, out errors)) return false;
        if (_authorDao.UpdateAuthor(author)) return true;
        
        errors.Add(new Error(ErrorType.Value, ErrorMessages.ErrorMessageAuthorAlreadyExist));
        return false;
    }

    public bool RemoveAuthor(int id, out List<Error> errors)
    {
        errors = new List<Error>();
        var author = GetAuthorById(id);
        if (author is null)
        {
            errors.Add(new Error(ErrorType.Value, ErrorMessages.ErrorMessagePolygraphyAuthorsNotExist));
            return false;
        }

        if (author.Id != id) return false;
        _authorDao.RemoveAuthor(id);
        return true;

    }

    public bool MarkAuthorAsDeleted(int id, out List<Error> errors)
    {
        errors = new List<Error>();
        var author = GetAuthorById(id);
        if (author is null)
        {
            errors.Add(new Error(ErrorType.Value, ErrorMessages.ErrorMessagePolygraphyAuthorsNotExist));
            return false;
        }

        if (author.Id != id) return false;
        _authorDao.MarkAuthorAsDeleted(id);
        return true;

    }

    public List<Author> GetAllAuthors()
    {
        return _authorDao.GetAllAuthors();
    }

    public List<Author> GetAuthorsByIds(List<int> authorIds)
    {
        return _authorDao.GetAuthorsByIds(authorIds);
    }

    public void ClearAuthors()
    {
        _authorDao.ClearAuthors();
    }

    public Author GetAuthorById(int id)
    {
        return _authorDao.GetAuthorById(id);
    }
}