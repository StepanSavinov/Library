using System;
using System.Collections.Generic;
using System.Linq;
using Epam.Library.BLL;
using Epam.Library.BLL.Interfaces;
using Epam.Library.DependencyConfig;
using Epam.Library.Entities;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Epam.Library.IntegrationTests;

public class LibraryLogicTests
{
    private ILibraryLogic _sut;
    private IAuthorLogic _authorLogic;
    private List<Error> _actualErrors;
    private List<Error> _expectedErrors;
    private List<Polygraphy> _expectedPolygraphies;
    private List<Polygraphy> _actualPolygraphies;

    [SetUp]
    public void Setup()
    {
        var services = new ServiceCollection();
        var registeredServices = Config.RegisterServices(services);
        _sut = registeredServices.GetService<ILibraryLogic>();
        _authorLogic = registeredServices.GetService<IAuthorLogic>();
        CreateAuthors();
    }

    [TearDown]
    public void Cleanup()
    {
        _sut.ClearLibrary();
        _sut.ClearNewspapers();
        _authorLogic.ClearAuthors();
    }

    [Test]
    public void AddToLibrary_Added()
    {
        // ARRANGE
        Book book = CreateBook();

        // ACT
        var result = _sut.AddToLibrary(book, out _actualErrors);

        // ASSERT
        Assert.IsTrue(result);
    }

    [Test]
    public void AddToLibrary_NotAdded_DueToPolygraphyIsNull()
    {
        // ARRANGE
        Book book = null;

        // ACT

        // ASSERT
        Assert.Throws<ArgumentNullException>(() => _sut.AddToLibrary(book, out _actualErrors));
    }

    [Test]
    public void AddToLibrary_NotAdded_DueToUniquenessError()
    {
        // ARRANGE
        CreateErrorLists();
        Book book1 = CreateBook();
        Book book2 = CreateBook();
        _expectedErrors.Add(new Error(ErrorType.Value, ErrorMessages.ErrorMessagePolygraphyAlreadyExist));

        // ACT
        _sut.AddToLibrary(book1, out _actualErrors);
        var result = _sut.AddToLibrary(book2, out _actualErrors);

        // ASSERT
        Assert.Multiple(() =>
        {
            Assert.IsFalse(result);
            Assert.IsTrue(_expectedErrors.OrderBy(e => e).SequenceEqual(_actualErrors.OrderBy(e => e)));
        });
    }

    [Test]
    public void AddToLibrary_NotAdded_DueToValidationError()
    {
        // ARRANGE
        CreateErrorLists();
        Book book = CreateBook();
        _expectedErrors.Add(new Error(ErrorType.Empty, ErrorMessages.ErrorMessagePolygraphyAuthorsEmpty));

        // ACT
        book.Authors.Clear();
        var result = _sut.AddToLibrary(book, out _actualErrors);

        // ASSERT
        Assert.Multiple(() =>
        {
            Assert.IsFalse(result);
            Assert.IsTrue(_expectedErrors.OrderBy(e => e.Type).SequenceEqual(_actualErrors.OrderBy(e => e.Type)));
        });
    }

    [Test]
    public void RemoveFromLibrary_Removed()
    {
        // ARRANGE
        Book book = CreateBook();

        // ACT
        _sut.AddToLibrary(book, out _actualErrors);
        var result = _sut.RemoveFromLibrary(book.Id, out _actualErrors);

        // ASSERT
        Assert.IsTrue(result);
    }

    [Test]
    public void RemoveFromLibrary_NotRemoved()
    {
        // ARRANGE
        CreateErrorLists();
        _expectedErrors.Add(new Error(ErrorType.Value, ErrorMessages.ErrorMessagePolygraphyNotExist));

        // ACT
        var result = _sut.RemoveFromLibrary(0, out _actualErrors);

        // ASSERT
        Assert.Multiple(() =>
        {
            Assert.IsTrue(_actualErrors.OrderBy(e => e).SequenceEqual(_expectedErrors.OrderBy(e => e)));
            Assert.IsFalse(result);
        });
    }

    [Test]
    public void GetPolygraphyById_Exists()
    {
        // ARRANGE
        Book book = CreateBook();

        // ACT
        _sut.AddToLibrary(book, out _actualErrors);
        var result = _sut.GetPolygraphyById(book.Id);

        // ASSERT

        Assert.AreEqual(book, result);
    }

    [Test]
    public void GetPolygraphyById_NotExists()
    {
        // ARRANGE
        Polygraphy poly;

        // ACT
        poly = _sut.GetPolygraphyById(0);

        // ASSERT
        Assert.IsNull(poly);
    }

    [Test]
    public void SearchByName_Success()
    {
        // ARRANGE
        CreatePolygraphiesLists();
        Book book = CreateBook();
        _expectedPolygraphies.Add(book);

        // ACT
        _sut.AddToLibrary(book, out _actualErrors);
        _actualPolygraphies = _sut.SearchByName(book.Name);

        // ASSERT
        Assert.IsTrue(_expectedPolygraphies.OrderBy(p => p.Name).SequenceEqual(_actualPolygraphies.OrderBy(p => p.Name)));
    }

    [Test]
    public void SearchByName_Failure()
    {
        // ARRANGE
        CreatePolygraphiesLists();
        Book book = CreateBook();

        // ACT
        _sut.AddToLibrary(book, out _actualErrors);
        _actualPolygraphies = _sut.SearchByName("Unexpected");

        // ASSERT
        CollectionAssert.IsEmpty(_actualPolygraphies);
    }

    [Test]
    public void GetSortedPolygraphies_Success()
    {
        // ARRANGE
        CreatePolygraphiesLists();
        Book book = CreateBook();
        Newspaper newspaper = CreateNewspaper();
        NewspaperIssue newspaperIssue = CreateNewspaperIssue();
        Patent patent = CreatePatent();

        _expectedPolygraphies.Add(book);
        _expectedPolygraphies.Add(newspaperIssue);
        _expectedPolygraphies.Add(patent);


        // ACT
        _sut.AddToLibrary(book, out _actualErrors);
        _sut.AddNewspaperToLibrary(newspaper, out _actualErrors);
        newspaperIssue.NewspaperId = newspaper.Id;
        _sut.AddToLibrary(newspaperIssue, out _actualErrors);
        _sut.AddToLibrary(patent, out _actualErrors);
        _actualPolygraphies = _sut.GetSortedPolygraphies(reverse: false);

        // ASSERT
        Assert.IsTrue(_expectedPolygraphies.SequenceEqual(_actualPolygraphies));
    }

    [Test]
    public void GetSortedPolygraphies_Success_Reversed()
    {
        // ARRANGE
        CreatePolygraphiesLists();
        Book book = CreateBook();
        Newspaper newspaper = CreateNewspaper();
        NewspaperIssue newspaperIssue = CreateNewspaperIssue();
        Patent patent = CreatePatent();
        _expectedPolygraphies.Add(patent);
        _expectedPolygraphies.Add(newspaperIssue);
        _expectedPolygraphies.Add(book);

        // ACT
        _sut.AddToLibrary(book, out _actualErrors);
        _sut.AddNewspaperToLibrary(newspaper, out _actualErrors);
        newspaperIssue.NewspaperId = newspaper.Id;
        _sut.AddToLibrary(newspaperIssue, out _actualErrors);
        _sut.AddToLibrary(patent, out _actualErrors);
        _actualPolygraphies = _sut.GetSortedPolygraphies(reverse: true);

        // ASSERT
        Assert.IsTrue(_expectedPolygraphies.SequenceEqual(_actualPolygraphies));
    }

    [Test]
    public void SearchByAuthor_Success()
    {
        // ARRANGE
        CreatePolygraphiesLists();
        Book book = CreateBook();
        Patent patent = CreatePatent();
        _expectedPolygraphies.Add(patent);
        _expectedPolygraphies.Add(book);

        // ACT
        _sut.AddToLibrary(book, out _actualErrors);
        _sut.AddToLibrary(patent, out _actualErrors);
        _actualPolygraphies = _sut.SearchByAuthor("Alexander", "Pushkin", PolygraphyEnum.PolyType.BookAndPatent);

        // ASSERT
        Assert.IsTrue(_expectedPolygraphies.OrderBy(p => p.Id).SequenceEqual(_actualPolygraphies.OrderBy(p => p.Id)));
    }

    [Test]
    public void SearchByAuthor_Failure()
    {
        // ARRANGE
        CreatePolygraphiesLists();
        Book book = CreateBook();
        Patent patent = CreatePatent();

        // ACT
        _sut.AddToLibrary(book, out _actualErrors);
        _sut.AddToLibrary(patent, out _actualErrors);
        _actualPolygraphies = _sut.SearchByAuthor("Fedor", "Dostoevsky", PolygraphyEnum.PolyType.BookAndPatent);

        // ASSERT
        CollectionAssert.IsEmpty(_actualErrors);
    }

    [Test]
    public void GetBooksByPublisher_Success()
    {
        // ARRANGE
        Dictionary<string, List<Book>> expectedDict = new Dictionary<string, List<Book>>();
        expectedDict.Add("Publisher", CreateListOfBooks());
        Dictionary<string, List<Book>> actualDict;

        // ACT
        foreach (var item in CreateListOfBooks())
        {
            _sut.AddToLibrary(item, out _actualErrors);
        }

        actualDict = _sut.GetBooksByPublisher("Publisher");

        // ASSERT
        Assert.AreEqual(expectedDict, actualDict);
    }

    [Test]
    public void GetBooksByPublisher_Failure()
    {
        // ARRANGE
        Dictionary<string, List<Book>> actualDict;

        // ACT
        foreach (var item in CreateListOfBooks())
        {
            _sut.AddToLibrary(item, out _actualErrors);
        }

        actualDict = _sut.GetBooksByPublisher("Wrongpublisher");

        // ASSERT
        CollectionAssert.IsEmpty(actualDict);
    }

    [Test]
    public void UpdatePolygraphy_Success()
    {
        // ARRANGE
        Book book = CreateBook();

        // ACT
        var result = _sut.UpdatePolygraphyInLibrary(book, out _actualErrors);

        // ASSERT
        Assert.IsTrue(result);
    }

    [Test]
    public void UpdatePolygraphy_Failure_DueToPolygraphyIsNull()
    {
        // ARRANGE
        Book book = null;

        // ACT

        // ASSERT
        Assert.Throws<ArgumentNullException>(() => _sut.UpdatePolygraphyInLibrary(book, out _actualErrors));
    }

    [Test]
    public void UpdatePolygraphy_Failure_DueToUniquenessError()
    {
        // ARRANGE
        CreateErrorLists();
        Book book1 = CreateBook();
        Book book2 = CreateBook();
        _expectedErrors.Add(new Error(ErrorType.Value, ErrorMessages.ErrorMessagePolygraphyAlreadyExist));

        // ACT
        _sut.AddToLibrary(book1, out _actualErrors);
        var result = _sut.UpdatePolygraphyInLibrary(book2, out _actualErrors);

        // ASSERT
        Assert.Multiple(() =>
        {
            Assert.IsFalse(result);
            Assert.IsTrue(_expectedErrors.OrderBy(e => e).SequenceEqual(_actualErrors.OrderBy(e => e)));
        });
    }

    [Test]
    public void UpdatePolygraphy_Failure_DueToValidationError()
    {
        // ARRANGE
        CreateErrorLists();
        Book book = CreateBook();
        _expectedErrors.Add(new Error(ErrorType.Empty, ErrorMessages.ErrorMessagePolygraphyAuthorsEmpty));

        // ACT
        book.Authors.Clear();
        var result = _sut.UpdatePolygraphyInLibrary(book, out _actualErrors);

        // ASSERT
        Assert.Multiple(() =>
        {
            Assert.IsFalse(result);
            Assert.IsTrue(_expectedErrors.OrderBy(e => e.Type).SequenceEqual(_actualErrors.OrderBy(e => e.Type)));
        });
    }

    [Test]
    public void GroupByYear_Success()
    {
        // ARRANGE
        Book book = CreateBook();
        Newspaper newspaper = CreateNewspaper();
        NewspaperIssue newspaperIssue = CreateNewspaperIssue();
        Patent patent = CreatePatent();
        Dictionary<int, List<Polygraphy>> expectedDict = new Dictionary<int, List<Polygraphy>>();
        expectedDict.Add(2007, new List<Polygraphy> {book});
        expectedDict.Add(2008, new List<Polygraphy> {newspaperIssue});
        expectedDict.Add(2009, new List<Polygraphy> {patent});
        Dictionary<int, List<Polygraphy>> actualDict;

        // ACT
        _sut.AddToLibrary(book, out _actualErrors);
        _sut.AddNewspaperToLibrary(newspaper, out _actualErrors);
        newspaperIssue.NewspaperId = newspaper.Id;
        _sut.AddToLibrary(newspaperIssue, out _actualErrors);
        _sut.AddToLibrary(patent, out _actualErrors);
        actualDict = _sut.GroupByYear();

        // ASSERT
        Assert.AreEqual(expectedDict, actualDict);
    }

    [Test]
    public void AddNewspaperToLibrary_Added()
    {
        // ARRANGE
        Newspaper newspaper = CreateNewspaper();
        // ACT
        var result = _sut.AddNewspaperToLibrary(newspaper, out _actualErrors);

        //ASSERT
        Assert.IsTrue(result);
    }

    [Test]
    public void AddNewspaperToLibrary_NotAdded_Null()
    {
        // ARRANGE
        Newspaper newspaper = null;

        // ACT

        //ASSERT
        Assert.Throws<ArgumentNullException>(() => _sut.AddNewspaperToLibrary(newspaper, out _actualErrors));
    }

    [Test]
    public void AddNewspaperToLibrary_NotAdded_DueToUniquenessError()
    {
        // ARRANGE
        CreateErrorLists();
        Newspaper newspaper1 = CreateNewspaper();
        Newspaper newspaper2 = CreateNewspaper();
        _expectedErrors.Add(new Error(ErrorType.Value, ErrorMessages.ErrorMessageNewspaperAlreadyExist));

        // ACT
        _sut.AddNewspaperToLibrary(newspaper1, out _actualErrors);
        var result = _sut.AddNewspaperToLibrary(newspaper2, out _actualErrors);

        // ASSERT
        Assert.Multiple(() =>
        {
            Assert.IsFalse(result);
            Assert.IsTrue(_expectedErrors.OrderBy(e => e).SequenceEqual(_actualErrors.OrderBy(e => e)));
        });
    }

    [Test]
    public void AddNewspaperToLibrary_NotAdded_DueToValidationError()
    {
        // ARRANGE
        CreateErrorLists();
        Newspaper newspaper = CreateNewspaper();
        _expectedErrors.Add(new Error(ErrorType.Empty, ErrorMessages.ErrorMessagePolygraphyNameEmpty));

        // ACT
        newspaper.Name = string.Empty;
        var result = _sut.AddNewspaperToLibrary(newspaper, out _actualErrors);

        // ASSERT
        Assert.Multiple(() =>
        {
            Assert.IsFalse(result);
            Assert.IsTrue(_expectedErrors.OrderBy(e => e.Type).SequenceEqual(_actualErrors.OrderBy(e => e.Type)));
        });
    }

    [Test]
    public void UpdateNewspaper_Success()
    {
        // ARRANGE
        Newspaper newspaper = CreateNewspaper();

        // ACT
        var result = _sut.UpdateNewspaperInLibrary(newspaper, out _actualErrors);

        // ASSERT
        Assert.IsTrue(result);
    }

    [Test]
    public void UpdateNewspaper_Failure_Null()
    {
        // ARRANGE
        Newspaper newspaper = null;

        // ACT

        // ASSERT
        Assert.Throws<ArgumentNullException>(() => _sut.UpdateNewspaperInLibrary(newspaper, out _actualErrors));
    }

    [Test]
    public void UpdateNewspaper_Failure_DueToUniquenessError()
    {
        // ARRANGE
        CreateErrorLists();
        Newspaper newspaper1 = CreateNewspaper();
        Newspaper newspaper2 = CreateNewspaper();
        _expectedErrors.Add(new Error(ErrorType.Value, ErrorMessages.ErrorMessageNewspaperAlreadyExist));

        // ACT
        _sut.AddNewspaperToLibrary(newspaper1, out _actualErrors);
        var result = _sut.UpdateNewspaperInLibrary(newspaper2, out _actualErrors);

        // ASSERT
        Assert.Multiple(() =>
        {
            Assert.IsFalse(result);
            Assert.IsTrue(_expectedErrors.OrderBy(e => e).SequenceEqual(_actualErrors.OrderBy(e => e)));
        });
    }

    [Test]
    public void UpdateNewspaper_Failure_DueToValidationError()
    {
        // ARRANGE
        CreateErrorLists();
        Newspaper newspaper = CreateNewspaper();
        _expectedErrors.Add(new Error(ErrorType.Empty, ErrorMessages.ErrorMessagePolygraphyNameEmpty));

        // ACT
        newspaper.Name = string.Empty;
        var result = _sut.UpdateNewspaperInLibrary(newspaper, out _actualErrors);

        // ASSERT
        Assert.Multiple(() =>
        {
            Assert.IsFalse(result);
            Assert.IsTrue(_expectedErrors.OrderBy(e => e.Type).SequenceEqual(_actualErrors.OrderBy(e => e.Type)));
        });
    }

    [Test]
    public void GetNewspaperById_Exists()
    {
        // ARRANGE
        Newspaper newspaper = CreateNewspaper();

        // ACT
        _sut.AddNewspaperToLibrary(newspaper, out _actualErrors);
        var result = _sut.GetNewspaperById(newspaper.Id);

        // ASSERT
        Assert.AreEqual(newspaper, result);
    }

    [Test]
    public void GetNewspaperById_NotExists()
    {
        // ARRANGE
        Newspaper newspaper;

        // ACT
        newspaper = _sut.GetNewspaperById(0);

        // ASSERT
        Assert.IsNull(newspaper);
    }

    [Test]
    public void RemoveNewspaperFromLibrary_Removed()
    {
        // ARRANGE
        CreateErrorLists();
        Newspaper newspaper = CreateNewspaper();

        // ACT
        _sut.AddNewspaperToLibrary(newspaper, out _actualErrors);
        var result = _sut.RemoveNewspaperFromLibrary(newspaper.Id, out _actualErrors);

        // ASSERT
        Assert.IsTrue(result);
    }

    [Test]
    public void RemoveNewspaperFromLibrary_NotRemoved()
    {
        // ARRANGE
        CreateErrorLists();
        _expectedErrors.Add(new Error(ErrorType.Value, ErrorMessages.ErrorMessageNewspaperNotExist));

        // ACT
        var result = _sut.RemoveNewspaperFromLibrary(0, out _actualErrors);

        // ASSERT
        Assert.Multiple(() =>
        {
            Assert.IsTrue(_actualErrors.OrderBy(e => e).SequenceEqual(_expectedErrors.OrderBy(e => e)));
            Assert.IsFalse(result);
        });
    }

    [Test]
    public void SearchNewspaperByName_Success()
    {
        // ARRANGE
        List<Newspaper> expectedNewspaperList = new List<Newspaper>();
        List<Newspaper> actualNewspaperList;
        Newspaper newspaper = CreateNewspaper();
        expectedNewspaperList.Add(newspaper);


        // ACT
        _sut.AddNewspaperToLibrary(newspaper, out _actualErrors);
        actualNewspaperList = _sut.SearchNewspaperByName(newspaper.Name);

        // ASSERT
        Assert.IsTrue(_expectedPolygraphies.OrderBy(p => p.Name).SequenceEqual(_actualPolygraphies.OrderBy(p => p.Name)));
    }

    [Test]
    public void SearchNewspaperByName_Failure()
    {
        // ARRANGE
        List<Newspaper> expectedNewspaperList = new List<Newspaper>();
        List<Newspaper> actualNewspaperList;
        Newspaper newspaper = CreateNewspaper();

        // ACT
        _sut.AddNewspaperToLibrary(newspaper, out _actualErrors);
        actualNewspaperList = _sut.SearchNewspaperByName("Unexpected");

        // ASSERT
        CollectionAssert.IsEmpty(actualNewspaperList);
    }
    
    private void CreateErrorLists()
    {
        _actualErrors = new List<Error>();
        _expectedErrors = new List<Error>();
    }

    private void CreatePolygraphiesLists()
    {
        _actualPolygraphies = new List<Polygraphy>();
        _expectedPolygraphies = new List<Polygraphy>();
    }

    private void CreateAuthors()
    {
        _authorLogic.AddAuthor(new Author("Alexander", "Pushkin"), out _actualErrors);
        _authorLogic.AddAuthor(new Author("Lev", "Tolstoi"), out _actualErrors);
    }

    private Book CreateBook() => new Book("Name1", new List<int>() {1, 2}, "City", "Publisher",
        new DateTime(2007, 10, 10), 13, "note", "0-545-01022-5");

    private List<Book> CreateListOfBooks()
    {
        Book book1 = new Book("Name1", new List<int>() {1, 2}, "City", "Publisher", new DateTime(2008, 10, 10), 13,
            "note", "0-545-01022-5");
        Book book2 = new Book("Name2", new List<int>() {1, 2}, "City", "Publisher", new DateTime(2009, 10, 10), 13,
            "note", "0-545-01022-6");
        Book book3 = new Book("Name3", new List<int>() {1, 2}, "City", "Publisher", new DateTime(2010, 10, 10), 13,
            "note", "0-545-01022-7");
        return new List<Book> {book1, book2, book3};
    }

    private NewspaperIssue CreateNewspaperIssue() =>
        new NewspaperIssue("Name2", 13, "City", "Publisher", new DateTime(2008, 10, 10), 20, "note");

    private Newspaper CreateNewspaper() => new Newspaper("NewspaperName", new List<int>(), "ISSN-6154-6253");

    private Patent CreatePatent() => new Patent("Name3", new List<int>() {1, 2}, "Country", 13,
        new DateTime(2009, 10, 10), new DateTime(2011, 11, 11), 20, "note");
}