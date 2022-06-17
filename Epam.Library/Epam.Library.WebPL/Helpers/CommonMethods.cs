namespace Epam.Library.WebPL.Helpers;

public class CommonMethods
{
    internal static List<T> CreatePagination<T>(IEnumerable<T> list, int page, int pageSize)
    {
        return list
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }
}