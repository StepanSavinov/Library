using Epam.Library.Entities;

namespace Epam.Library.DAL.Interfaces;

public interface IAuthorDao
{
    bool AddAuthor(Author author);
    void RemoveAuthor(int id);
    List<Author> GetAllAuthors();
    List<Author> GetAuthorsByIds(List<int> authorIds);
    Author GetAuthorById(int id);
    void ClearAuthors();
    bool UpdateAuthor(Author author);
    void MarkAuthorAsDeleted(int id);
}