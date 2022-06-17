using System;
using System.Collections.Generic;
using System.Linq;
using Epam.Library.BLL;
using Epam.Library.BLL.Interfaces;
using Epam.Library.Entities;
using NUnit.Framework;

namespace Epam.Library.UnitTests;

public class AuthorValidatorTests
{
    private IValidatable<Author> _sut;
    private List<Error> _expectedErrors;
    private List<Error> _actualErrors;

    [OneTimeSetUp]
    public void Setup()
    {
        _sut = new AuthorValidator();
    }

    public void CreateErrorLists()
    {
        _actualErrors = new List<Error>();
        _expectedErrors = new List<Error>();
    }

    [Test]
    [TestCase("Alexander", "Pushkin")]
    [TestCase("Алексёндр", "Пушкён")]
    [TestCase("Alexander", "de Pushkin")]
    [TestCase("Alexander", "de-Pushkin")]
    [TestCase("Alexan-Der", "de-Pushkin")]
    [TestCase("Alexander", "o'Pushkin")]
    public void IsValid_True(string firstname, string lastname)
    {
        // ARRANGE
        Author author = new Author(firstname, lastname);

        // ACT
        bool result = _sut.IsValid(author, out _actualErrors);

        // ASSERT
        Assert.IsTrue(result);
    }

    [Test]
    public void IsValid_False_DueToAuthorIsNull()
    {
        // ARRANGE
        Author author = null;

        //ACT

        // ASSERT
        Assert.Throws<ArgumentNullException>(() => _sut.IsValid(author, out _actualErrors));
    }

    [Test]
    public void IsValid_False_DueToFirstnameLastnameIsEmpty()
    {
        // ARRANGE
        CreateErrorLists();
        _expectedErrors.Add(new Error(ErrorType.Empty, ErrorMessages.ErrorMessageAuthorFirstnameEmpty));
        _expectedErrors.Add(new Error(ErrorType.Empty, ErrorMessages.ErrorMessageAuthorLastnameEmpty));
        Author author = new Author(String.Empty, String.Empty);

        // ACT
        bool result = _sut.IsValid(author, out _actualErrors);

        // ASSERT
        Assert.Multiple(() =>
        {
            Assert.IsFalse(result);
            Assert.IsTrue(Enumerable.SequenceEqual(_expectedErrors.OrderBy(e => e.Message),
                _actualErrors.OrderBy(e => e.Message)));
        });
    }

    [Test]
    public void IsValid_False_DueToFirstnameIsEmpty()
    {
        // ARRANGE
        CreateErrorLists();
        _expectedErrors.Add(new Error(ErrorType.Empty, ErrorMessages.ErrorMessageAuthorFirstnameEmpty));
        Author author = new Author(string.Empty, "Pushkin");

        //ACT
        bool result = _sut.IsValid(author, out _actualErrors);

        // ASSERT
        Assert.Multiple(() =>
        {
            Assert.IsTrue(Enumerable.SequenceEqual(_expectedErrors.OrderBy(e => e), _actualErrors.OrderBy(e => e)));
            Assert.IsFalse(result);
        });
    }

    [Test]
    [TestCase("alexander", "Pushkin")]
    [TestCase("ALEXANDER", "Pushkin")]
    [TestCase("-Alexander", "Pushkin")]
    [TestCase("Alexander-", "Pushkin")]
    [TestCase("Alexan-der", "Pushkin")]
    public void IsValid_False_DueToFirstnameIncorrectFormat(string firstname, string lastname)
    {
        // ARRANGE
        CreateErrorLists();
        _expectedErrors.Add(new Error(ErrorType.Format, ErrorMessages.ErrorMessageAuthorFirstnameIncorrect));
        Author author = new Author(firstname, lastname);

        // ACT
        bool result = _sut.IsValid(author, out _actualErrors);

        // ASSERT
        Assert.Multiple(() =>
        {
            Assert.IsTrue(Enumerable.SequenceEqual(_expectedErrors.OrderBy(e => e), _actualErrors.OrderBy(e => e)));
            Assert.IsFalse(result);
        });
    }

    [Test]
    public void IsValid_False_DueToFirstnameTooLong()
    {
        // ARRANGE
        CreateErrorLists();
        _expectedErrors.Add(new Error(ErrorType.Length, ErrorMessages.ErrorMessageAuthorFirstnameTooLong));
        Author author = new Author($"A{new string('a', 50)}", "Pushkin");

        // ACT
        bool result = _sut.IsValid(author, out _actualErrors);

        // ASSERT

        Assert.Multiple(() =>
        {
            Assert.IsTrue(Enumerable.SequenceEqual(_expectedErrors.OrderBy(e => e), _actualErrors.OrderBy(e => e)));
            Assert.IsFalse(result);
        });
    }

    [Test]
    public void IsValid_False_DueToLastnameIsEmpty()
    {
        // ARRANGE
        CreateErrorLists();
        _expectedErrors.Add(new Error(ErrorType.Empty, ErrorMessages.ErrorMessageAuthorLastnameEmpty));
        Author author = new Author("Alexander", string.Empty);

        // ACT
        bool result = _sut.IsValid(author, out _actualErrors);

        // ASSERT
        Assert.Multiple(() =>
        {
            Assert.IsTrue(Enumerable.SequenceEqual(_expectedErrors.OrderBy(e => e), _actualErrors.OrderBy(e => e)));
            Assert.IsFalse(result);
        });
    }

    [Test]
    [TestCase("Alexander", "pushkin")]
    [TestCase("Alexander", "PUSHKIN")]
    [TestCase("Alexander", "de pushkin")]
    [TestCase("Alexander", "De Pushkin")]
    [TestCase("Alexander", "Push-kin")]
    [TestCase("Alexander", "'Pushkin")]
    [TestCase("Alexander", "Pushkin'")]
    public void IsValid_False_DueToLastnameIncorrectFormat(string firstname, string lastname)
    {
        // ARRANGE

        CreateErrorLists();
        _expectedErrors.Add(new Error(ErrorType.Format, ErrorMessages.ErrorMessageAuthorLastnameIncorrect));
        Author author = new Author(firstname, lastname);

        // ACT
        bool result = _sut.IsValid(author, out _actualErrors);

        // ASSERT
        foreach (var item in _actualErrors)
        {
            System.Console.WriteLine(item.Message);
        }

        Assert.Multiple(() =>
        {
            Assert.IsTrue(Enumerable.SequenceEqual(_expectedErrors.OrderBy(e => e), _actualErrors.OrderBy(e => e)));
            Assert.IsFalse(result);
        });
    }

    [Test]
    public void IsValid_False_DueToLastnameTooLong()
    {
        // ARRANGE
        CreateErrorLists();
        _expectedErrors.Add(new Error(ErrorType.Length, ErrorMessages.ErrorMessageAuthorLastnameTooLong));
        Author author = new Author("Alexander", $"A{new string('a', 200)}");

        // ACT
        bool result = _sut.IsValid(author, out _actualErrors);

        // ASSERT
        Assert.Multiple(() =>
        {
            Assert.IsTrue(Enumerable.SequenceEqual(_expectedErrors.OrderBy(e => e), _actualErrors.OrderBy(e => e)));
            Assert.IsFalse(result);
        });
    }

    [Test]
    public void IsValid_False_DueToAllPropertiesIsEmpty()
    {
        // ARRANGE
        CreateErrorLists();
        _expectedErrors.Add(new Error(ErrorType.Empty, ErrorMessages.ErrorMessageAuthorFirstnameEmpty));
        _expectedErrors.Add(new Error(ErrorType.Empty, ErrorMessages.ErrorMessageAuthorLastnameEmpty));
        Author author = new Author(string.Empty, string.Empty);

        // ACT
        bool result = _sut.IsValid(author, out _actualErrors);

        // ASSERT
        Assert.Multiple(() =>
        {
            Assert.IsTrue(Enumerable
                .SequenceEqual(_expectedErrors.OrderBy(e => e.Message), _actualErrors.OrderBy(e => e.Message)));
            Assert.IsFalse(result);
        });
    }
}