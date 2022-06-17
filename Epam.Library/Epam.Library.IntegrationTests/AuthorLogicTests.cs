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

public class AuthorLogicTests
{
    private IAuthorLogic _sut;
    private List<Error> _actualErrors;
    private List<Error> _expectedErrors;

    [SetUp]
    public void Setup()
    {
        var services = new ServiceCollection();
        _sut = Config.RegisterServices(services).GetService<IAuthorLogic>();
    }

    [TearDown]
    public void Cleanup()
    {
        _sut.ClearAuthors();
    }

    [Test]
    public void AddAuthor_Added()
    {
        // ARRANGE
        Author author = new Author("Alexander", "Pushkin");

        // ACT
        var result = _sut.AddAuthor(author, out _actualErrors);

        // ASSERT
        Assert.IsTrue(result);
    }

    [Test]
    public void AddAuthor_NotAdded_DueToAuthorIsNull()
    {
        // ARRANGE
        Author author = null;

        // ACT

        // ASSERT
        Assert.Throws<ArgumentNullException>(() => _sut.AddAuthor(author, out _actualErrors));
    }

    [Test]
    public void AddAuthor_NotAdded_DueToUniquenessError()
    {
        // ARRANGE
        CreateErrorLists();
        Author author1 = new Author("Alexander", "Pushkin");
        Author author2 = new Author("Alexander", "Pushkin");
        _expectedErrors.Add(new Error(ErrorType.Value, ErrorMessages.ErrorMessageAuthorAlreadyExist));

        // ACT
        _sut.AddAuthor(author1, out _actualErrors);
        var result = _sut.AddAuthor(author2, out _actualErrors);


        // ASSERT
        Assert.Multiple(() =>
        {
            Assert.IsFalse(result);
            Assert.IsTrue(_expectedErrors.OrderBy(e => e).SequenceEqual(_actualErrors.OrderBy(e => e)));
        });
    }

    [Test]
    public void AddAuthor_NotAdded_DueToValidationError()
    {
        // ARRANGE
        CreateErrorLists();
        Author author = new Author("alexander", "Pushkin");
        _expectedErrors.Add(new Error(ErrorType.Format, ErrorMessages.ErrorMessageAuthorFirstnameIncorrect));

        // ACT
        var result = _sut.AddAuthor(author, out _actualErrors);


        // ASSERT
        Assert.Multiple(() =>
        {
            Assert.IsFalse(result);
            Assert.IsTrue(_expectedErrors.OrderBy(e => e).SequenceEqual(_actualErrors.OrderBy(e => e)));
        });
    }

    [Test]
    public void UpdateAuthor_Success()
    {
        // ARRANGE
        Author author = new Author("Alexander", "Pushkin");

        // ACT
        var result = _sut.UpdateAuthor(author, out _actualErrors);

        // ASSERT
        Assert.IsTrue(result);
    }

    [Test]
    public void UpdateAuthor_Failure_DueToAuthorIsNull()
    {
        // ARRANGE
        Author? author = null;

        // ACT

        // ASSERT
        Assert.Throws<ArgumentNullException>(() => _sut.UpdateAuthor(author, out _actualErrors));
    }

    [Test]
    public void UpdateAuthor_Failure_DueToUniquenessError()
    {
        // ARRANGE
        CreateErrorLists();
        Author author1 = new Author("Alexander", "Pushkin");
        Author author2 = new Author("Alexander", "Pushkin");
        _expectedErrors.Add(new Error(ErrorType.Value, ErrorMessages.ErrorMessageAuthorAlreadyExist));

        // ACT
        _sut.AddAuthor(author1, out _actualErrors);
        var result = _sut.UpdateAuthor(author2, out _actualErrors);


        // ASSERT
        Assert.Multiple(() =>
        {
            Assert.IsFalse(result);
            Assert.IsTrue(_expectedErrors.OrderBy(e => e).SequenceEqual(_actualErrors.OrderBy(e => e)));
        });
    }

    [Test]
    public void UpdateAuthor_Failure_DueToValidationError()
    {
        // ARRANGE
        CreateErrorLists();
        Author author = new Author("alexander", "Pushkin");
        _expectedErrors.Add(new Error(ErrorType.Format, ErrorMessages.ErrorMessageAuthorFirstnameIncorrect));

        // ACT
        var result = _sut.UpdateAuthor(author, out _actualErrors);


        // ASSERT
        Assert.Multiple(() =>
        {
            Assert.IsFalse(result);
            Assert.IsTrue(_expectedErrors.OrderBy(e => e).SequenceEqual(_actualErrors.OrderBy(e => e)));
        });
    }

    [Test]
    public void RemoveAuthor_Removed()
    {
        // ARRANGE
        Author author = new Author("Alexander", "Pushkin");

        // ACT
        _sut.AddAuthor(author, out _actualErrors);
        var result = _sut.RemoveAuthor(author.Id, out _actualErrors);

        // ASSERT
        Assert.IsTrue(result);
    }

    [Test]
    public void RemoveAuthor_NotRemoved()
    {
        // ARRANGE
        CreateErrorLists();
        _expectedErrors.Add(new Error(ErrorType.Value, ErrorMessages.ErrorMessagePolygraphyAuthorsNotExist));

        // ACT
        var result = _sut.RemoveAuthor(0, out _actualErrors);

        // ASSERT
        Assert.Multiple(() =>
        {
            Assert.IsTrue(_actualErrors.OrderBy(e => e).SequenceEqual(_expectedErrors.OrderBy(e => e)));
            Assert.IsFalse(result);
        });
    }

    [Test]
    public void GetAuthorById_Exists()
    {
        // ARRANGE
        Author author = new Author("Alexander", "Pushkin");

        // ACT
        _sut.AddAuthor(author, out _actualErrors);
        var result = _sut.GetAuthorById(author.Id);

        // ASSERT
        Assert.AreEqual(author, result);
    }

    [Test]
    public void GetAuthorById_NotExists()
    {
        // ARRANGE

        // ACT
        var result = _sut.GetAuthorById(0);

        // ASSERT
        Assert.IsNull(result);
    }

    private void CreateErrorLists()
    {
        _actualErrors = new List<Error>();
        _expectedErrors = new List<Error>();
    }
}