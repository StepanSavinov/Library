namespace Epam.Library.WebPL.Models;

public class PageViewModel
{
    public int FirstPage => 1;
    public int PageNumber { get; }
    public int TotalPages { get; }
 
    public PageViewModel(int count, int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
    }

    public int CountPreviousPages => PageNumber - FirstPage;
    public int CountNextPages => TotalPages - PageNumber;
 
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}