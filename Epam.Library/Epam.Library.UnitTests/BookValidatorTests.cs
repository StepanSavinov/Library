using System;
using System.Collections.Generic;
using System.Linq;
using Epam.Library.BLL;
using Epam.Library.BLL.Interfaces;
using Epam.Library.Entities;
using Moq;
using NUnit.Framework;

namespace Epam.Library.UnitTests;

public class BookValidatorTests
{
    private IValidatable<Book> _sut;
    private Mock<IAuthorLogic> _authorLogicMock;
    private List<Error> _expectedErrors;
    private List<Error> _actualErrors;
    private List<Author> _authors;

    [OneTimeSetUp]
    public void Setup()
    {
        _authorLogicMock = new Mock<IAuthorLogic>();
        _authors = new List<Author>()
        {
            new Author("Alexander", "Pushkin") {Id = 1},
            new Author("Lev", "Tolstoi") {Id = 2},
            new Author("Fedor", "Dostoevsky") {Id = 3},
            new Author("Aleksei", "Tolstoi") {Id = 4}
        };
        _authorLogicMock.Setup(author => author.GetAllAuthors()).Returns(_authors);
        _authorLogicMock.Setup(author => author.GetAuthorsByIds(new List<int> {1, 2})).Returns(new List<Author>()
        {
            new Author("Alexander", "Pushkin") {Id = 1},
            new Author("Lev", "Tolstoi") {Id = 2}
        });
        _sut = new BookValidator(_authorLogicMock.Object);
    }

    private void CreateErrorLists()
    {
        _actualErrors = new List<Error>();
        _expectedErrors = new List<Error>();
    }

    [Test]
    [TestCase("Ростов")]
    [TestCase("Rostov")]
    public void IsValid_True(string city)
    {
        // ARRANGE
        CreateErrorLists();
        Book book = new Book("Name", new List<int>() {1, 2}, city, "Publisher", new DateTime(2010, 10, 10), 13,
            "note", "0-545-01022-5");

        // ACT
        bool result = _sut.IsValid(book, out _actualErrors);

        // ASSERT
        Assert.IsTrue(result);
    }

    [Test]
    public void IsValid_False_DueToBookIsNull()
    {
        // ARRANGE
        Book book = null;

        //ACT

        // ASSERT
        Assert.Throws<ArgumentNullException>(() => _sut.IsValid(book, out _actualErrors));
    }

    [Test]
    public void IsValid_False_DueToAllPropertiesIsIncorrect()
    {
        // ARRANGE
        CreateErrorLists();
        _expectedErrors.Add(new Error(ErrorType.Empty, ErrorMessages.ErrorMessagePolygraphyNameEmpty));
        _expectedErrors.Add(new Error(ErrorType.Empty, ErrorMessages.ErrorMessagePolygraphyCityEmpty));
        _expectedErrors.Add(new Error(ErrorType.Empty, ErrorMessages.ErrorMessagePolygraphyAuthorsEmpty));
        _expectedErrors.Add(new Error(ErrorType.Value, ErrorMessages.ErrorMessagePolygraphyPagesNegative));
        _expectedErrors.Add(new Error(ErrorType.Value, ErrorMessages.ErrorMessagePolygraphyDateTooEarly));
        _expectedErrors.Add(new Error(ErrorType.Length, ErrorMessages.ErrorMessagePolygraphyFootnoteTooLong));
        _expectedErrors.Add(new Error(ErrorType.Empty, ErrorMessages.ErrorMessagePolygraphyPublisherEmpty));
        _expectedErrors.Add(new Error(ErrorType.Format, ErrorMessages.ErrorMessagePolygraphyIsbnIncorrect));

        Book book = new Book(String.Empty, new List<int>(), String.Empty, String.Empty, new DateTime(1399, 10, 10),
            -20, $"{new string('a', 2001)}", "2562");

        // ACT
        bool result = _sut.IsValid(book, out _actualErrors);

        // ASSERT
        Assert.Multiple(() =>
        {
            Assert.IsTrue(Enumerable.SequenceEqual(_expectedErrors.OrderBy(e => e.Message),
                _actualErrors.OrderBy(e => e.Message)));
            Assert.IsFalse(result);
        });
    }

    [Test]
    public void IsValid_False_DueToBookNameIsEmpty()
    {
        // ARRANGE
        CreateErrorLists();
        _expectedErrors.Add(new Error(ErrorType.Empty, ErrorMessages.ErrorMessagePolygraphyNameEmpty));
        Book book = new Book(String.Empty, new List<int>() {1, 2}, "City", "Publisher", new DateTime(2010, 10, 10),
            13, "note", "0-545-01022-5");


        // ACT
        bool result = _sut.IsValid(book, out _actualErrors);

        // ASSERT
        Assert.Multiple(() =>
        {
            Assert.IsTrue(Enumerable.SequenceEqual(_expectedErrors.OrderBy(e => e), _actualErrors.OrderBy(e => e)));
            Assert.IsFalse(result);
        });
    }

    [Test]
    public void IsValid_False_DueToBookNameIsTooLong()
    {
        // ARRANGE
        CreateErrorLists();
        _expectedErrors.Add(new Error(ErrorType.Length, ErrorMessages.ErrorMessagePolygraphyNameTooLong));
        Book book = new Book($"A{new string('a', 300)}", new List<int>() {1, 2}, "City", "Publisher",
            new DateTime(2010, 10, 10), 13, "note", "0-545-01022-5");

        // ACT
        bool result = _sut.IsValid(book, out _actualErrors);

        // ASSERT
        Assert.Multiple(() =>
        {
            Assert.IsTrue(Enumerable.SequenceEqual(_expectedErrors.OrderBy(e => e), _actualErrors.OrderBy(e => e)));
            Assert.IsFalse(result);
        });
    }

    [Test]
    public void IsValid_False_DueToAuthorsListIsEmpty()
    {
        // ARRANGE
        CreateErrorLists();
        _expectedErrors.Add(new Error(ErrorType.Empty, ErrorMessages.ErrorMessagePolygraphyAuthorsEmpty));
        Book book = new Book("Name", new List<int>(), "City", "Publisher", new DateTime(2017, 10, 10), 13, "note",
            "0-545-01022-5");

        // ACT
        bool result = _sut.IsValid(book, out _actualErrors);

        // ASSERT
        Assert.Multiple(() =>
        {
            Assert.IsTrue(Enumerable.SequenceEqual(_expectedErrors.OrderBy(e => e), _actualErrors.OrderBy(e => e)));
            Assert.IsFalse(result);
        });
    }

    [Test]
    public void IsValid_False_DueToAuthorsDontExist()
    {
        // ARRANGE
        CreateErrorLists();
        _expectedErrors.Add(new Error(ErrorType.Value, ErrorMessages.ErrorMessagePolygraphyAuthorsNotExist));
        Book book = new Book("Name", new List<int>() {65, 38}, "City", "Publisher", new DateTime(2010, 10, 10), 13,
            "note", "0-545-01022-5");

        // ACT
        _authorLogicMock.Setup(author => author.GetAuthorsByIds(book.Authors)).Returns(new List<Author>());
        bool result = _sut.IsValid(book, out _actualErrors);

        // ASSERT
        Assert.Multiple(() =>
        {
            Assert.IsTrue(Enumerable.SequenceEqual(_expectedErrors.OrderBy(e => e), _actualErrors.OrderBy(e => e)));
            Assert.IsFalse(result);
        });
    }

    [Test]
    public void IsValid_False_DueToCreationTimeIsTooEarly()
    {
        // ARRANGE
        CreateErrorLists();
        _expectedErrors.Add(new Error(ErrorType.Value, ErrorMessages.ErrorMessagePolygraphyDateTooEarly));
        Book book = new Book("Name", new List<int>() {1, 2}, "City", "Publisher", new DateTime(1398, 10, 10), 13,
            "note", "0-545-01022-5");

        // ACT
        bool result = _sut.IsValid(book, out _actualErrors);

        // ASSERT
        Assert.Multiple(() =>
        {
            Assert.IsTrue(Enumerable.SequenceEqual(_expectedErrors.OrderBy(e => e), _actualErrors.OrderBy(e => e)));
            Assert.IsFalse(result);
        });
    }

    [Test]
    public void IsValid_False_DueToCreationTimeIsFuture()
    {
        // ARRANGE
        CreateErrorLists();
        _expectedErrors.Add(new Error(ErrorType.Value, ErrorMessages.ErrorMessagePolygraphyDateFuture));
        Book book = new Book("Name", new List<int>() {1, 2}, "City", "Publisher", new DateTime(2498, 10, 10), 13,
            "note", "0-545-01022-5");

        // ACT
        bool result = _sut.IsValid(book, out _actualErrors);

        // ASSERT
        Assert.Multiple(() =>
        {
            Assert.IsTrue(Enumerable.SequenceEqual(_expectedErrors.OrderBy(e => e), _actualErrors.OrderBy(e => e)));
            Assert.IsFalse(result);
        });
    }

    [Test]
    public void IsValid_False_DueToTotalPagesIsNegative()
    {
        // ARRANGE
        CreateErrorLists();
        _expectedErrors.Add(new Error(ErrorType.Value, ErrorMessages.ErrorMessagePolygraphyPagesNegative));
        Book book = new Book("Name", new List<int>() {1, 2}, "City", "Publisher", new DateTime(1687, 10, 10), -13,
            "note", "0-545-01022-5");

        // ACT
        bool result = _sut.IsValid(book, out _actualErrors);

        // ASSERT
        Assert.Multiple(() =>
        {
            Assert.IsTrue(Enumerable.SequenceEqual(_expectedErrors.OrderBy(e => e), _actualErrors.OrderBy(e => e)));
            Assert.IsFalse(result);
        });
    }

    [Test]
    public void IsValid_False_DueToFootnoteIsTooLong()
    {
        // ARRANGE
        CreateErrorLists();
        _expectedErrors.Add(new Error(ErrorType.Length, ErrorMessages.ErrorMessagePolygraphyFootnoteTooLong));
        Book book = new Book("Name", new List<int>() {1, 2}, "City", "Publisher", new DateTime(1798, 10, 10), 13,
            $"{new string('a', 2001)}", "0-545-01022-5");

        // ACT
        bool result = _sut.IsValid(book, out _actualErrors);

        // ASSERT
        Assert.Multiple(() =>
        {
            Assert.IsTrue(Enumerable.SequenceEqual(_expectedErrors.OrderBy(e => e), _actualErrors.OrderBy(e => e)));
            Assert.IsFalse(result);
        });
    }

    [Test]
    public void IsValid_False_DueToCityIsEmpty()
    {
        // ARRANGE
        CreateErrorLists();
        _expectedErrors.Add(new Error(ErrorType.Empty, ErrorMessages.ErrorMessagePolygraphyCityEmpty));
        Book book = new Book("Name", new List<int>() {1, 2}, string.Empty, "Publisher", new DateTime(2010, 10, 10),
            13, "note", "0-545-01022-5");

        // ACT
        bool result = _sut.IsValid(book, out _actualErrors);

        // ASSESRT
        Assert.Multiple(() =>
        {
            Assert.IsTrue(Enumerable.SequenceEqual(_expectedErrors.OrderBy(e => e), _actualErrors.OrderBy(e => e)));
            Assert.IsFalse(result);
        });
    }

    [Test]
    [TestCase("rostov")]
    [TestCase("rostov on don")]
    [TestCase("rostov-on-don")]
    [TestCase("ростов")]
    [TestCase("ростов на дону")]
    [TestCase("ростов-на-дону")]
    public void IsValid_False_DueToCityIsIncorrect(string city)
    {
        // ARRANGE
        CreateErrorLists();
        _expectedErrors.Add(new Error(ErrorType.Format, ErrorMessages.ErrorMessagePolygraphyCityIncorrect));
        Book book = new Book("Name", new List<int>() {1, 2}, city, "Publisher", new DateTime(2010, 10, 10), 13,
            "note", "0-545-01022-5");

        // ACT
        bool result = _sut.IsValid(book, out _actualErrors);

        // ASSERT
        Assert.Multiple(() =>
        {
            Assert.IsTrue(Enumerable.SequenceEqual(_expectedErrors.OrderBy(e => e), _actualErrors.OrderBy(e => e)));
            Assert.IsFalse(result);
        });
    }

    [Test]
    public void IsValid_False_DueToCityIsTooLong()
    {
        // ARRANGE
        CreateErrorLists();
        _expectedErrors.Add(new Error(ErrorType.Length, ErrorMessages.ErrorMessagePolygraphyCityTooLong));
        Book book = new Book("Name", new List<int>() {1, 2}, $"S{new string('a', 200)}", "Publisher",
            DateTime.Parse("1798-10-10"), 13, "note", "0-545-01022-5");

        // ACT
        bool result = _sut.IsValid(book, out _actualErrors);

        // ASSERT
        Assert.Multiple(() =>
        {
            Assert.IsTrue(Enumerable.SequenceEqual(_expectedErrors.OrderBy(e => e), _actualErrors.OrderBy(e => e)));
            Assert.IsFalse(result);
        });
    }

    [Test]
    public void IsValid_False_DueToPublisherIsEmpty()
    {
        // ARRANGE
        CreateErrorLists();
        _expectedErrors.Add(new Error(ErrorType.Empty, ErrorMessages.ErrorMessagePolygraphyPublisherEmpty));
        Book book = new Book("Name", new List<int>() {1, 2}, "City", String.Empty, new DateTime(2010, 10, 10), 13,
            "note", "0-545-01022-5");

        // ACT
        bool result = _sut.IsValid(book, out _actualErrors);

        // ASSERT
        Assert.Multiple(() =>
        {
            Assert.IsTrue(Enumerable.SequenceEqual(_expectedErrors.OrderBy(e => e), _actualErrors.OrderBy(e => e)));
            Assert.IsFalse(result);
        });
    }

    [Test]
    public void IsValid_False_DueToPublisherIsTooLong()
    {
        // ARRANGE
        CreateErrorLists();
        _expectedErrors.Add(new Error(ErrorType.Length, ErrorMessages.ErrorMessagePolygraphyPublisherTooLong));
        Book book = new Book("Name", new List<int>() {1, 2}, "City", $"P{new string('a', 300)}",
            new DateTime(2010, 10, 10), 13, "note", "0-545-01022-5");

        // ACT
        bool result = _sut.IsValid(book, out _actualErrors);

        // ASSERT
        Assert.Multiple(() =>
        {
            Assert.IsTrue(Enumerable.SequenceEqual(_expectedErrors.OrderBy(e => e), _actualErrors.OrderBy(e => e)));
            Assert.IsFalse(result);
        });
    }

    [Test]
    [TestCase("76436")]
    public void IsValid_False_DueToIsbnIsIncorrect(string isbn)
    {
        // ARRANGE
        CreateErrorLists();
        _expectedErrors.Add(new Error(ErrorType.Format, ErrorMessages.ErrorMessagePolygraphyIsbnIncorrect));
        Book book = new Book("Name", new List<int>() {1, 2}, "City", "Publisher", new DateTime(2010, 10, 10), 13,
            "note", isbn);

        // ACT
        bool result = _sut.IsValid(book, out _actualErrors);

        // ASSERT
        Assert.Multiple(() =>
        {
            Assert.IsTrue(Enumerable.SequenceEqual(_expectedErrors.OrderBy(e => e), _actualErrors.OrderBy(e => e)));
            Assert.IsFalse(result);
        });
    }
}