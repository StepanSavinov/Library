using Epam.Library.Entities;

namespace Epam.Library.BLL.Interfaces;

public interface ILibraryLogic
{
    bool AddToLibrary(Polygraphy polygraphy, out List<Error> errors);
    bool RemoveFromLibrary(int id, out List<Error> errors);
    bool MarkPolygraphyAsDeleted(int id);
    Polygraphy GetPolygraphyById(int id);
    List<Polygraphy> GetAllLibrary();
    List<NewspaperIssue> GetAllNewspaperIssuesByNewspaperId(int id);
    List<Polygraphy> SearchByName(string name);
    List<Polygraphy> GetSortedPolygraphies(bool reverse);
    List<Polygraphy> SearchByAuthor(string firstname, string lastname, PolygraphyEnum.PolyType type);
    Dictionary<string, List<Book>> GetBooksByPublisher(string publisher);
    Dictionary<int, List<Polygraphy>> GroupByYear();
    bool UpdatePolygraphyInLibrary(Polygraphy polygraphy, out List<Error> errors);
    bool AddNewspaperToLibrary(Newspaper newspaper, out List<Error> errors);
    bool UpdateNewspaperInLibrary(Newspaper newspaper, out List<Error> errors);
    bool RemoveNewspaperFromLibrary(int id, out List<Error> errors);
    Newspaper GetNewspaperById(int id);
    List <Newspaper> GetAllNewspapers();
    List<Newspaper> SearchNewspaperByName(string name);
    void ClearLibrary();
    void ClearNewspapers();
}