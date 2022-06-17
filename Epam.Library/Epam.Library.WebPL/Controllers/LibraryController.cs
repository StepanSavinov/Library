using AutoMapper;
using Epam.Library.BLL.Interfaces;
using Epam.Library.Entities;
using Epam.Library.WebPL.Filters;
using Epam.Library.WebPL.Helpers;
using Epam.Library.WebPL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Epam.Library.WebPL.Controllers;

public class LibraryController : Controller
{
    private readonly IMapper _mapper;
    private readonly IAuthorLogic _authorLogic;
    private readonly ILibraryLogic _libraryLogic;
    private const int PageSize = 20;

    public LibraryController(IMapper mapper, IAuthorLogic authorLogic, ILibraryLogic libraryLogic)
    {
        _mapper = mapper;
        _authorLogic = authorLogic;
        _libraryLogic = libraryLogic;
    }
    
    [ServiceFilter(typeof(ExceptionFilter))]
    [AllowAnonymous]
    [HttpGet]
    public IActionResult Index() => View();
    
    [ServiceFilter(typeof(ExceptionFilter))]
    [AllowAnonymous]
    [HttpGet]
    public IActionResult GetAllLibrary(int page = 1)
    {
        var mappedPolygraphies = MapPolygraphyList(_libraryLogic.GetAllLibrary());
        var items = CommonMethods.CreatePagination(mappedPolygraphies, page, PageSize);
        
        var pageViewModel = new PageViewModel(mappedPolygraphies.Count, page, PageSize);
        var viewModel = new PolygraphyPageViewModel
        {
            PageViewModel = pageViewModel,
            Polygraphies = items
        };

        return View(viewModel);
    }
    
    [HttpGet]
    [AllowAnonymous]
    [ServiceFilter(typeof(ExceptionFilter))]
    public IActionResult GetSortedPolygraphies(bool reversed, int page = 1)
    {
        var polygraphies = MapPolygraphyList(_libraryLogic.GetSortedPolygraphies(reversed));
        var items = CommonMethods.CreatePagination(polygraphies, page, PageSize);
        
        var pageViewModel = new PageViewModel(polygraphies.Count, page, PageSize);
        var viewModel = new PolygraphyPageViewModel
        {
            PageViewModel = pageViewModel,
            Polygraphies = items
        };
    
        return View("GetAllLibrary", viewModel);
    }
    
    [HttpGet]
    [AllowAnonymous]
    [ServiceFilter(typeof(ExceptionFilter))]
    public IActionResult GroupByYear(int page = 1)
    {
        var polygraphies = _libraryLogic.GroupByYear();
        var items = CommonMethods.CreatePagination(polygraphies, page, PageSize);

        var pageViewModel = new PageViewModel(polygraphies.Values.Count, page, PageSize);
        var viewModel = new PolygraphyPageViewModel
        {
            PageViewModel = pageViewModel,
            GroupedByYear = items
        };

        return View(viewModel);
    }
    
    [ServiceFilter(typeof(ExceptionFilter))]
    [HttpGet]
    [Authorize(Policy = "User")]
    public IActionResult SearchByName() => View();
    
    [ServiceFilter(typeof(ExceptionFilter))]
    [Authorize(Policy = "User")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SearchByName(SearchByNameViewModel searchViewModel, int page = 1)
    {
        if (!ModelState.IsValid) return View(searchViewModel);
        var polygraphies = MapPolygraphyList(_libraryLogic.SearchByName(searchViewModel.Name));
        var items = CommonMethods.CreatePagination(polygraphies, page, PageSize);
        
        var pageViewModel = new PageViewModel(polygraphies.Count, page, PageSize);
        var viewModel = new PolygraphyPageViewModel
        {
            PageViewModel = pageViewModel,
            Polygraphies = items
        };
        
        return View("GetAllLibrary", viewModel);
    }
    
    [ServiceFilter(typeof(ExceptionFilter))]
    [Authorize(Policy = "User")]
    public IActionResult SearchByAuthor() => View();
    
    [ServiceFilter(typeof(ExceptionFilter))]
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "User")]
    public IActionResult SearchByAuthor(SearchByAuthorViewModel searchViewModel, int page = 1)
    {
        if (!ModelState.IsValid) return View(searchViewModel);
        var polygraphies = MapPolygraphyList(_libraryLogic.SearchByAuthor(
            searchViewModel.Firstname, 
            searchViewModel.Lastname, 
            searchViewModel.Type));
        
        var items = CommonMethods.CreatePagination(polygraphies, page, PageSize);
        
        var pageViewModel = new PageViewModel(polygraphies.Count, page, PageSize);
        var viewModel = new PolygraphyPageViewModel
        {
            PageViewModel = pageViewModel,
            Polygraphies = items
        };
        
        return View("GetAllLibrary", viewModel);
    }
    
    [ServiceFilter(typeof(ExceptionFilter))]
    [HttpGet]
    [Authorize(Policy = "User")]
    public IActionResult SearchBooksByPublisher() => View();

    [ServiceFilter(typeof(ExceptionFilter))]
    [Authorize(Policy = "User")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SearchBooksByPublisher(SearchByPublisherViewModel searchViewModel, int page = 1)
    {
        if (!ModelState.IsValid) return View(searchViewModel);
        var polygraphies = _libraryLogic.GetBooksByPublisher(searchViewModel.Publisher);
        var items = CommonMethods.CreatePagination(polygraphies, page, PageSize);
        
        var pageViewModel = new PageViewModel(polygraphies.Values.Count, page, PageSize);
        var viewModel = new PolygraphyPageViewModel
        {
            PageViewModel = pageViewModel,
            BooksByPublisher = items
        };
        
        return View("GetBooksByPublisher", viewModel);
    }
    
    [ServiceFilter(typeof(ExceptionFilter))]
    [Authorize(Policy = "User")]
    public IActionResult GetAllNewspaperIssuesById(int id, int page = 1)
    {
        var issues = MapIssuesList(_libraryLogic.GetAllNewspaperIssuesByNewspaperId(id));
        var items = CommonMethods.CreatePagination(issues, page, PageSize);
        
        var pageViewModel = new PageViewModel(issues.Count, page, PageSize);
        var viewModel = new PolygraphyPageViewModel
        {
            PageViewModel = pageViewModel,
            Issues = items
        };
        
        return View("GetAllNewspaperIssuesById", viewModel);
    }
    
    [ServiceFilter(typeof(ExceptionFilter))]
    [Authorize(Policy = "User")]
    public IActionResult AddAuthorToLibrary() => View();

    [ServiceFilter(typeof(ExceptionFilter))]
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "Librarian")]
    public IActionResult AddAuthorToLibrary(AuthorViewModel viewModel)
    {
        if (!ModelState.IsValid) return View(viewModel);

        if (_authorLogic.AddAuthor(new Author(viewModel.Firstname, viewModel.Lastname), out var errors))
        {
            return View(viewModel);
        }

        foreach (var error in errors)
        {
            ModelState.AddModelError(string.Empty, error.Message);
        }
        return View(viewModel);
    }
    
    [ServiceFilter(typeof(ExceptionFilter))]
    [AllowAnonymous]
    public IActionResult GetAllAuthors(int page = 1)
    {
        var authors = _authorLogic.GetAllAuthors();
        var items = CommonMethods.CreatePagination(authors, page, PageSize);

        var pageViewModel = new PageViewModel(authors.Count, page, PageSize);
        var viewModel = new AuthorsPageViewModel
        {
            PageViewModel = pageViewModel,
            Authors = items
        };
        
        return View(viewModel);
    }

    [ServiceFilter(typeof(ExceptionFilter))]
    [Authorize(Policy = "Librarian")]
    public IActionResult AddNewspaperToLibrary() => View();

    [ServiceFilter(typeof(ExceptionFilter))]
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "Librarian")]
    public IActionResult AddNewspaperToLibrary(NewspaperViewModel viewModel)
    {
        if (!ModelState.IsValid) return View(viewModel);

        if (_libraryLogic.AddNewspaperToLibrary(new Newspaper(viewModel.Name, new List<int>(), viewModel.ISSN), out var errors))
        {
            return View(viewModel);
        }

        foreach (var error in errors)
        {
            ModelState.AddModelError(string.Empty, error.Message);
        }
        return View(viewModel);
    }

    [ServiceFilter(typeof(ExceptionFilter))]
    [Authorize(Policy = "Librarian")]
    [HttpGet]
    public IActionResult AddBookToLibrary()
    {
        ViewBag.Authors = _authorLogic.GetAllAuthors();
        return View();
    }

    [ServiceFilter(typeof(ExceptionFilter))]
    [Authorize(Policy = "Librarian")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AddBookToLibrary(BookViewModel viewModel)
    {
        if (!ModelState.IsValid) return View(viewModel);

        Polygraphy poly = new Book(
            viewModel.Name,
            viewModel.Authors,
            viewModel.City,
            viewModel.Publisher,
            viewModel.Created,
            viewModel.TotalPages,
            viewModel.Footnote,
            viewModel.ISBN);
        ViewBag.Authors = _authorLogic.GetAllAuthors();
        if (_libraryLogic.AddToLibrary(poly, out var errors))
        {
            return View(viewModel);
        }

        foreach (var error in errors)
        {
            ModelState.AddModelError(string.Empty, error.Message);
        }

        return View(viewModel);
    }

    [ServiceFilter(typeof(ExceptionFilter))]
    [Authorize(Policy = "Librarian")]
    [HttpGet]
    public IActionResult AddNewspaperIssueToLibrary()
    {
        ViewBag.Newspapers = _libraryLogic.GetAllNewspapers();
        return View();
    }

    [ServiceFilter(typeof(ExceptionFilter))]
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "Librarian")]
    public IActionResult AddNewspaperIssueToLibrary(NewspaperIssueViewModel viewModel)
    {
        if (!ModelState.IsValid) return View(viewModel);
        
        Polygraphy poly = new NewspaperIssue(
                viewModel.Name,
                viewModel.Number,
                viewModel.City,
                viewModel.Publisher,
                viewModel.Created,
                viewModel.TotalPages,
                viewModel.Footnote)
            {NewspaperId = viewModel.NewspaperId};
        ViewBag.Newspapers = _libraryLogic.GetAllNewspapers();
        if (_libraryLogic.AddToLibrary(poly, out var errors))
        {
            return View(viewModel);
        }

        foreach (var error in errors)
        {
            ModelState.AddModelError(string.Empty, error.Message);
        }
        return View(viewModel);
    }
    
    [ServiceFilter(typeof(ExceptionFilter))]
    [HttpGet]
    [Authorize(Policy = "Librarian")]
    public IActionResult AddPatentToLibrary()
    {
        ViewBag.Authors = _authorLogic.GetAllAuthors();
        return View();
    }

    [ServiceFilter(typeof(ExceptionFilter))]
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "Librarian")]
    public IActionResult AddPatentToLibrary(PatentViewModel viewModel)
    {
        if (!ModelState.IsValid) return View(viewModel);

        Polygraphy poly = new Patent(
            viewModel.Name,
            viewModel.Authors,
            viewModel.Country,
            viewModel.Number,
            viewModel.Created,
            viewModel.Published,
            viewModel.TotalPages,
            viewModel.Footnote);
        
        ViewBag.Authors = _authorLogic.GetAllAuthors();
        if (_libraryLogic.AddToLibrary(poly, out var errors))
        {
            return View(viewModel);
        }

        foreach (var error in errors)
        {
            ModelState.AddModelError(string.Empty, error.Message);
        }

        return View(viewModel);
    }
    
    [ServiceFilter(typeof(ExceptionFilter))]
    [Authorize(Policy = "Librarian")]
    public IActionResult UpdateAuthor(int id, AuthorViewModel viewModel)
    {
        if (!ModelState.IsValid) return View(viewModel);
        var author = _authorLogic.GetAuthorById(id);
        author.Firstname = viewModel.Firstname;
        author.Lastname = viewModel.Lastname;
        if (_authorLogic.UpdateAuthor(author, out var errors))
        {
            return RedirectToAction("GetAllAuthors");
        }

        foreach (var error in errors)
        {
            ModelState.AddModelError(string.Empty, error.Message);
        }

        return View(viewModel);
    }
    
    [ServiceFilter(typeof(ExceptionFilter))]
    [ServiceFilter(typeof(ActionFilter))]
    [Authorize(Policy = "Admin")]
    public IActionResult DeleteAuthor(int id)
    {
        _authorLogic.RemoveAuthor(id, out _);
        return RedirectToAction("GetAllAuthors");
    }
    
    [ServiceFilter(typeof(ExceptionFilter))]
    [Authorize(Policy = "Librarian")]
    public IActionResult MarkAuthorAsDeleted(int id)
    {
        _authorLogic.MarkAuthorAsDeleted(id, out _);
        return RedirectToAction("GetAllAuthors");
    }
    
    [ServiceFilter(typeof(ExceptionFilter))]
    [ServiceFilter(typeof(ActionFilter))]
    [Authorize(Policy = "Admin")]
    public IActionResult RemoveFromLibrary(int id)
    {
        _libraryLogic.RemoveFromLibrary(id, out _);
        return RedirectToAction("GetAllLibrary");
    }

    [ServiceFilter(typeof(ExceptionFilter))]
    [Authorize(Policy = "Librarian")]
    public IActionResult MarkPolygraphyAsDeleted(int id)
    {
        _libraryLogic.MarkPolygraphyAsDeleted(id);
        return RedirectToAction("GetAllLibrary");
    }

    [ServiceFilter(typeof(ExceptionFilter))]
    [HttpGet]
    [Authorize(Policy = "Librarian")]
    public IActionResult UpdateBookInLibrary()
    {
        ViewBag.Authors = _authorLogic.GetAllAuthors();
        return View();
    }
    
    [ServiceFilter(typeof(ExceptionFilter))]
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "Librarian")]
    public IActionResult UpdateBookInLibrary(int id, BookViewModel viewModel)
    {
        if (!ModelState.IsValid) return View(viewModel);
        var book = _libraryLogic.GetPolygraphyById(id) as Book;
        
        book.Name = viewModel.Name;
        book.Authors = viewModel.Authors;
        book.City = viewModel.City;
        book.Publisher = viewModel.Publisher;
        book.Created = viewModel.Created;
        book.Footnote = viewModel.Footnote;
        book.ISBN = viewModel.ISBN;
        book.TotalPages = viewModel.TotalPages;
        ViewBag.Authors = _authorLogic.GetAllAuthors();
        
        if (_libraryLogic.UpdatePolygraphyInLibrary(book, out var errors))
        {
            return RedirectToAction("GetAllLibrary");
        }

        foreach (var error in errors)
        {
            ModelState.AddModelError(string.Empty, error.Message);
        }

        return View(viewModel);
    }

    [ServiceFilter(typeof(ExceptionFilter))]
    [HttpGet]
    [Authorize(Policy = "Librarian")]
    public IActionResult UpdateNewspaperIssueInLibrary()
    {
        ViewBag.Newspapers = _libraryLogic.GetAllNewspapers();
        return View();
    } 
    
    [ServiceFilter(typeof(ExceptionFilter))]
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "Librarian")]
    public IActionResult UpdateNewspaperIssueInLibrary(int id, NewspaperIssueViewModel viewModel)
    {
        if (!ModelState.IsValid) return View(viewModel);
        var newspaperIssue = _libraryLogic.GetPolygraphyById(id) as NewspaperIssue;
        
        newspaperIssue.Name = viewModel.Name;
        newspaperIssue.Number = viewModel.Number;
        newspaperIssue.City = viewModel.City;
        newspaperIssue.Publisher = viewModel.Publisher;
        newspaperIssue.Created = viewModel.Created;
        newspaperIssue.Footnote = viewModel.Footnote;
        newspaperIssue.TotalPages = viewModel.TotalPages;
        newspaperIssue.NewspaperId = viewModel.NewspaperId;
        ViewBag.Newspapers = _libraryLogic.GetAllNewspapers();
        
        if (_libraryLogic.UpdatePolygraphyInLibrary(newspaperIssue, out var errors))
        {
            return RedirectToAction("GetAllLibrary");
        }

        foreach (var error in errors)
        {
            ModelState.AddModelError(string.Empty, error.Message);
        }

        return View(viewModel);
    }
    
    [ServiceFilter(typeof(ExceptionFilter))]
    [HttpGet]
    [Authorize(Policy = "Librarian")]
    public IActionResult UpdatePatentInLibrary()
    {
        ViewBag.Authors = _authorLogic.GetAllAuthors();
        return View();
    }
    
    [ServiceFilter(typeof(ExceptionFilter))]
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "Librarian")]
    public IActionResult UpdatePatentInLibrary(int id, PatentViewModel viewModel)
    {
        if (!ModelState.IsValid) return View(viewModel);
        var patent = _libraryLogic.GetPolygraphyById(id) as Patent;
        
        patent.Name = viewModel.Name;
        patent.Authors = viewModel.Authors;
        patent.Country = viewModel.Country;
        patent.Number = viewModel.Number;
        patent.Created = viewModel.Created;
        patent.Footnote = viewModel.Footnote;
        patent.Published = viewModel.Published;
        patent.TotalPages = viewModel.TotalPages;
        ViewBag.Authors = _authorLogic.GetAllAuthors();
        
        if (_libraryLogic.UpdatePolygraphyInLibrary(patent, out var errors))
        {
            return RedirectToAction("GetAllLibrary");
        }

        foreach (var error in errors)
        {
            ModelState.AddModelError(string.Empty, error.Message);
        }

        return View(viewModel);
    }

    private List<PolygraphyReadViewModel> MapPolygraphyList(List<Polygraphy> polygraphies)
    {
        var mappedPolygraphies = new List<PolygraphyReadViewModel>();
        foreach (var polygraphy in polygraphies)
        {
            switch (polygraphy)
            {
                case Book book:
                {
                    var mappedBook = _mapper.Map<BookReadViewModel>(book);
                    mappedBook.Authors = _authorLogic.GetAuthorsByIds(book.Authors);
                    mappedPolygraphies.Add(mappedBook);
                    break;
                }
                case NewspaperIssue newspaperIssue:
                {
                    var mappedIssue = _mapper.Map<NewspaperIssueReadViewModel>(newspaperIssue);
                    mappedIssue.ISSN = _libraryLogic.GetNewspaperById(newspaperIssue.NewspaperId).ISSN;
                    mappedPolygraphies.Add(mappedIssue);
                    break;
                }
                case Patent patent:
                {
                    var mappedPatent = _mapper.Map<PatentReadViewModel>(patent);
                    mappedPatent.Authors = _authorLogic.GetAuthorsByIds(patent.Authors);
                    mappedPolygraphies.Add(mappedPatent);
                    break;
                }
            }
        }

        return mappedPolygraphies;
    }

    private List<NewspaperIssueReadViewModel> MapIssuesList(List<NewspaperIssue> issues)
    {
        var mappedIssues = new List<NewspaperIssueReadViewModel>();
        foreach (var newspaperIssue in issues)
        {
            var mappedIssue = _mapper.Map<NewspaperIssueReadViewModel>(newspaperIssue);
            mappedIssue.ISSN = _libraryLogic.GetNewspaperById(newspaperIssue.NewspaperId).ISSN;
            mappedIssues.Add(mappedIssue);
        }

        return mappedIssues;
    }
}