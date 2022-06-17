using Epam.Library.Entities;

namespace Epam.Library.DAL.Interfaces;

public interface ILibraryDao
{
    bool AddToLibrary(Polygraphy polygraphy);
    void RemoveFromLibrary(int id);
    void MarkPolygraphyAsDeleted(int id);
    Polygraphy GetPolygraphyById(int id);
    List<Polygraphy> GetAllLibrary();
    List<NewspaperIssue> GetAllNewspaperIssuesByNewspaperId(int id);
    List<Polygraphy> SearchByName(string name);
    List<Polygraphy> GetSortedPolygraphies(bool reverse);
    List<Polygraphy> SearchByAuthor(string firstname, string lastname, PolygraphyEnum.PolyType type);
    void ClearLibrary();
    bool UpdatePolygraphyInLibrary(Polygraphy polygraphy);
    List<Book> GetAllBooks();
    bool AddNewspaperToLibrary(Newspaper newspaper);
    bool UpdateNewspaperInLibrary(Newspaper newspaper);
    void RemoveNewspaperFromLibrary(int id);
    Newspaper GetNewspaperById(int id);
    List<Newspaper> GetAllNewspapers();
    List<Newspaper> SearchNewspaperByName(string name);
    void ClearNewspapers();

}