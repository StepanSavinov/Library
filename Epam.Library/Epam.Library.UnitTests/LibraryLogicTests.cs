using System;
using System.Collections.Generic;
using Epam.Library.BLL;
using Epam.Library.BLL.Interfaces;
using Epam.Library.DAL.Interfaces;
using Epam.Library.Entities;
using Moq;
using NUnit.Framework;

namespace Epam.Library.UnitTests;

public class LibraryLogicTests
{
    private ILibraryLogic _sut;
    private Mock<ILibraryDao> _libraryDaoMock;
    private Mock<IValidatable<Book>> _bookValidatorMock;
    private Mock<IValidatable<NewspaperIssue>> _newspaperIssueValidatorMock;
    private Mock<IValidatable<Newspaper>> _newspaperValidatorMock;
    private Mock<IValidatable<Patent>> _patentValidatorMock;
    private List<Error> _actualErrors;
    private List<Error> _expectedErrors;

    [OneTimeSetUp]
    public void Setup()
    {
        _libraryDaoMock = new Mock<ILibraryDao>();
        _bookValidatorMock = new Mock<IValidatable<Book>>();
        _newspaperIssueValidatorMock = new Mock<IValidatable<NewspaperIssue>>();
        _newspaperValidatorMock = new Mock<IValidatable<Newspaper>>();
        _patentValidatorMock = new Mock<IValidatable<Patent>>();
        _sut = new LibraryLogic(
            _libraryDaoMock.Object,
            _bookValidatorMock.Object,
            _patentValidatorMock.Object,
            _newspaperIssueValidatorMock.Object,
            _newspaperValidatorMock.Object
        );
    }

    private void CreateErrorLists()
    {
        _expectedErrors = new List<Error>();
        _actualErrors = new List<Error>();
    }

    [Test]
    public void AddToLibrary_Added()
    {
        // ARRANGE
        CreateErrorLists();
        Polygraphy book = new Book
        (
            "Name",
            new List<int>() {1, 2},
            "City",
            "Publisher",
            new DateTime(2010, 10, 10),
            13,
            "note",
            "0-545-01022-5"
        );
        _libraryDaoMock.Setup(mock => mock.AddToLibrary(book)).Returns(true);
        _bookValidatorMock.Setup(poly => poly.IsValid((Book) book, out _actualErrors)).Returns(true);

        // ACT
        bool result = _sut.AddToLibrary(book, out _actualErrors);

        // ASSERT
        Assert.IsTrue(result);
    }

    [Test]
    public void AddToLibrary_NotAddedDueToValidationError()
    {
        // ARRANGE
        CreateErrorLists();
        Book book = new Book
        (
            "Name",
            new List<int>() {1, 2},
            "City",
            "Publisher",
            new DateTime(2010, 10, 10),
            13,
            "note",
            "0-545-01022-5"
        );
        _libraryDaoMock.Setup(mock => mock.AddToLibrary(book)).Returns(false);
        _bookValidatorMock.Setup(poly => poly.IsValid(book, out _actualErrors)).Returns(false);

        // ACT
        bool result = _sut.AddToLibrary(book, out _actualErrors);

        // ASSERT
        Assert.IsFalse(result);
    }

    [Test]
    public void AddToLibrary_NotAddedDueToUniquenessError()
    {
        // ARRANGE
        CreateErrorLists();
        Book book = new Book
        (
            "Name",
            new List<int>() {1, 2},
            "City",
            "Publisher",
            new DateTime(2010, 10, 10),
            13,
            "note",
            "0-545-01022-5"
        );
        _libraryDaoMock.Setup(mock => mock.AddToLibrary(book)).Returns(false);
        _bookValidatorMock.Setup(poly => poly.IsValid(book, out _actualErrors)).Returns(true);

        // ACT
        bool result = _sut.AddToLibrary(book, out _actualErrors);

        // ASSERT
        Assert.IsFalse(result);
    }

    [Test]
    public void RemoveFromLibrary_Removed()
    {
        // ARRANGE
        Polygraphy book = new Book("Name", new List<int>() {1, 2}, "City", "Publisher", new DateTime(2010, 10, 10), 13,
            "note", "0-545-01022-5") {Id = 1};
        _libraryDaoMock.Setup(mock => mock.RemoveFromLibrary(1));
        _libraryDaoMock.Setup(mock => mock.GetPolygraphyById(1)).Returns(book);

        // ACT
        bool removed = _sut.RemoveFromLibrary(1, out List<Error> errors);

        // ASSERT
        Assert.IsTrue(removed);
    }

    [Test]
    public void RemoveFromLibrary_NotRemoved()
    {
        // ARRANGE
        Polygraphy book = new Book("Name", new List<int>() {1, 2}, "City", "Publisher", new DateTime(2010, 10, 10), 13,
            "note", "0-545-01022-5") {Id = 1};
        _libraryDaoMock.Setup(mock => mock.RemoveFromLibrary(2));
        _libraryDaoMock.Setup(mock => mock.GetPolygraphyById(2)).Returns((Book) null);

        // ACT
        bool removed = _sut.RemoveFromLibrary(2, out List<Error> errors);

        // ASSERT
        Assert.IsFalse(removed);
    }
}