using Epam.Library.Entities;

namespace Epam.Library.BLL.Interfaces;

public interface IAuthorLogic
{
    bool AddAuthor(Author author, out List<Error> errors);
    bool RemoveAuthor(int id, out List<Error> errors);
    List<Author> GetAllAuthors();
    List<Author> GetAuthorsByIds(List<int> authorIds);
    Author GetAuthorById(int id);
    void ClearAuthors();
    bool UpdateAuthor(Author author, out List<Error> errors);
    bool MarkAuthorAsDeleted(int id, out List<Error> errors);
}