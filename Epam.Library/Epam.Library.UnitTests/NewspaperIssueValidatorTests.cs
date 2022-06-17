using System;
using System.Collections.Generic;
using System.Linq;
using Epam.Library.BLL;
using Epam.Library.BLL.Interfaces;
using Epam.Library.Entities;
using NUnit.Framework;

namespace Epam.Library.UnitTests;

public class NewspaperIssueValidatorTests
{
    private IValidatable<NewspaperIssue> _sut;
    //private Mock<ILibraryLogic> _libraryLogicMock;
    private List<Error> _expectedErrors;
    private List<Error> _actualErrors;

    [OneTimeSetUp]
    public void Setup()
    {
        //_libraryLogicMock = new Mock<ILibraryLogic>();
        _sut = new NewspaperIssueValidator();
    }

    public void CreateErrorLists()
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
        NewspaperIssue newspaper =
            new NewspaperIssue("Name", 13, city, "Publisher", new DateTime(2010, 10, 10), 20, "note");

        // ACT
        bool result = _sut.IsValid(newspaper, out _actualErrors);

        // ASSERT
        Assert.IsTrue(result);
    }

    [Test]
    public void IsValid_False_DueToNewspaperIssueIsNull()
    {
        // ARRANGE
        NewspaperIssue newspaper = null;

        //ACT

        // ASSERT
        Assert.Throws<ArgumentNullException>(() => _sut.IsValid(newspaper, out _actualErrors));
    }

    [Test]
    public void IsValid_False_DueToAllPropertiesIsIncorrect()
    {
        // ARRANGE
        CreateErrorLists();
        _expectedErrors.Add(new Error(ErrorType.Empty, ErrorMessages.ErrorMessagePolygraphyNameEmpty));
        _expectedErrors.Add(new Error(ErrorType.Empty, ErrorMessages.ErrorMessagePolygraphyCityEmpty));
        _expectedErrors.Add(new Error(ErrorType.Value, ErrorMessages.ErrorMessagePolygraphyNumberNegative));
        _expectedErrors.Add(new Error(ErrorType.Value, ErrorMessages.ErrorMessagePolygraphyPagesNegative));
        _expectedErrors.Add(new Error(ErrorType.Value, ErrorMessages.ErrorMessagePatentNewspaperDateTooEarly));
        _expectedErrors.Add(new Error(ErrorType.Length, ErrorMessages.ErrorMessagePolygraphyFootnoteTooLong));
        _expectedErrors.Add(new Error(ErrorType.Empty, ErrorMessages.ErrorMessagePolygraphyPublisherEmpty));

        NewspaperIssue newspaper = new NewspaperIssue(string.Empty, -13, string.Empty, string.Empty,
            new DateTime(1400, 10, 10), -20, $"{new string('a', 2001)}");

        // ACT
        bool result = _sut.IsValid(newspaper, out _actualErrors);

        // ASSERT
        Assert.Multiple(() =>
        {
            Assert.IsTrue(Enumerable.SequenceEqual(_expectedErrors.OrderBy(e => e.Message),
                _actualErrors.OrderBy(e => e.Message)));
            Assert.IsFalse(result);
        });
    }

    [Test]
    public void IsValid_False_DueToNewspaperNameIsEmpty()
    {
        // ARRANGE
        CreateErrorLists();
        _expectedErrors.Add(new Error(ErrorType.Empty, ErrorMessages.ErrorMessagePolygraphyNameEmpty));
        NewspaperIssue newspaper = new NewspaperIssue(String.Empty, 13, "City", "Publisher", new DateTime(2010, 10, 10),
            20, "note");

        // ACT
        bool result = _sut.IsValid(newspaper, out _actualErrors);

        // ASSERT
        Assert.Multiple(() =>
        {
            Assert.IsTrue(Enumerable.SequenceEqual(_expectedErrors.OrderBy(e => e), _actualErrors.OrderBy(e => e)));
            Assert.IsFalse(result);
        });
    }

    [Test]
    public void IsValid_False_DueToNewspaperNameIsTooLong()
    {
        // ARRANGE
        CreateErrorLists();
        _expectedErrors.Add(new Error(ErrorType.Length, ErrorMessages.ErrorMessagePolygraphyNameTooLong));
        NewspaperIssue newspaper = new NewspaperIssue($"A{new string('a', 300)}", 13, "City", "Publisher",
            new DateTime(2010, 10, 10), 20, "note");

        // ACT
        bool result = _sut.IsValid(newspaper, out _actualErrors);

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
        _expectedErrors.Add(new Error(ErrorType.Value, ErrorMessages.ErrorMessagePatentNewspaperDateTooEarly));
        NewspaperIssue newspaper =
            new NewspaperIssue("Name", 13, "City", "Publisher", new DateTime(1470, 10, 10), 20, "note");

        // ACT
        bool result = _sut.IsValid(newspaper, out _actualErrors);

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
        NewspaperIssue newspaper =
            new NewspaperIssue("Name", 13, "City", "Publisher", new DateTime(2023, 10, 10), 20, "note");

        // ACT
        bool result = _sut.IsValid(newspaper, out _actualErrors);

        // ASSERT
        Assert.Multiple(() =>
        {
            Assert.IsTrue(Enumerable.SequenceEqual(_expectedErrors.OrderBy(e => e), _actualErrors.OrderBy(e => e)));
            Assert.IsFalse(result);
        });
    }

    [Test]
    public void IsValid_False_DueToNumberIsNegative()
    {
        // ARRANGE
        CreateErrorLists();
        _expectedErrors.Add(new Error(ErrorType.Value, ErrorMessages.ErrorMessagePolygraphyNumberNegative));
        NewspaperIssue newspaper =
            new NewspaperIssue("Name", -13, "City", "Publisher", new DateTime(2010, 10, 10), 20, "note");

        // ACT
        bool result = _sut.IsValid(newspaper, out _actualErrors);

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
        NewspaperIssue newspaper = new NewspaperIssue("Name", 13, String.Empty, "Publisher", new DateTime(2010, 10, 10),
            20, "note");

        // ACT
        bool result = _sut.IsValid(newspaper, out _actualErrors);

        // ASSESRT
        Assert.Multiple(() =>
        {
            Assert.IsTrue(Enumerable.SequenceEqual(_expectedErrors.OrderBy(e => e), _actualErrors.OrderBy(e => e)));
            Assert.IsFalse(result);
        });
    }

    [Test]
    [TestCase("rostov")]
    public void IsValid_False_DueToCityIsIncorrect(string city)
    {
        // ARRANGE
        CreateErrorLists();
        _expectedErrors.Add(new Error(ErrorType.Format, ErrorMessages.ErrorMessagePolygraphyCityIncorrect));
        NewspaperIssue newspaper =
            new NewspaperIssue("Name", 13, city, "Publisher", new DateTime(2010, 10, 10), 20, "note");

        // ACT
        bool result = _sut.IsValid(newspaper, out _actualErrors);

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
        NewspaperIssue newspaper = new NewspaperIssue("Name", 13, $"S{new string('a', 200)}", "Publisher",
            new DateTime(2010, 10, 10), 20, "note");

        // ACT
        bool result = _sut.IsValid(newspaper, out _actualErrors);

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
        NewspaperIssue newspaper =
            new NewspaperIssue("Name", 13, "City", String.Empty, new DateTime(2010, 10, 10), 20, "note");

        // ACT
        bool result = _sut.IsValid(newspaper, out _actualErrors);

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
        NewspaperIssue newspaper = new NewspaperIssue("Name", 13, "City", $"P{new string('a', 300)}",
            new DateTime(2010, 10, 10), 20, "note");

        // ACT
        bool result = _sut.IsValid(newspaper, out _actualErrors);

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
        NewspaperIssue newspaper =
            new NewspaperIssue("Name", 13, "City", "Publisher", new DateTime(2010, 10, 10), -20, "note");

        // ACT
        bool result = _sut.IsValid(newspaper, out _actualErrors);

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
        NewspaperIssue newspaper = new NewspaperIssue("Name", 13, "City", "Publisher", new DateTime(2010, 10, 10), 20,
            $"{new string('a', 2001)}");

        // ACT
        bool result = _sut.IsValid(newspaper, out _actualErrors);

        // ASSERT
        Assert.Multiple(() =>
        {
            Assert.IsTrue(Enumerable.SequenceEqual(_expectedErrors.OrderBy(e => e), _actualErrors.OrderBy(e => e)));
            Assert.IsFalse(result);
        });
    }
}