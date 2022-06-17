using System.Collections.Generic;
using System.Linq;
using Epam.Library.BLL;
using Epam.Library.BLL.Interfaces;
using Epam.Library.DAL.Interfaces;
using Epam.Library.Entities;
using NUnit.Framework;
using Moq;

namespace Epam.Library.UnitTests;

public class AuthorLogicTests
{
    private IAuthorLogic _sut;
    private Mock<IAuthorDao> _authorDAOMock;
    private Mock<IValidatable<Author>> _authorValidatorMock;
    private List<Error> _expectedErrors;
    private List<Error> _actualErrors;

    [OneTimeSetUp]
    public void Setup()
    {
        _authorDAOMock = new Mock<IAuthorDao>();
        _authorValidatorMock = new Mock<IValidatable<Author>>();
        _sut = new AuthorLogic(_authorDAOMock.Object, _authorValidatorMock.Object);
    }

    [Test]
    public void AddAuthor_Added()
    {
        // ARRANGE
        _authorDAOMock.Setup(mock => mock.AddAuthor(It.IsAny<Author>())).Returns(true);
        _authorValidatorMock.Setup(author => author.IsValid(It.IsAny<Author>(), out _actualErrors)).Returns(true);


        // ACT
        bool added = _sut.AddAuthor(It.IsAny<Author>(), out _actualErrors);

        //ASSERT
        Assert.IsTrue(added);
    }

    [Test]
    public void AddAuthor_NotAdded_DueToValidationError()
    {
        // ARRANGE
        CreateErrorLists();
        _authorDAOMock.Setup(mock => mock.AddAuthor(It.IsAny<Author>())).Returns(false);
        _authorValidatorMock.Setup(author => author.IsValid(It.IsAny<Author>(), out _actualErrors)).Returns(false);


        // ACT
        bool added = _sut.AddAuthor(It.IsAny<Author>(), out _actualErrors);

        //ASSERT
        Assert.IsFalse(added);
    }

    [Test]
    public void AddAuthor_NotAdded_DueToUniquenessError()
    {
        // ARRANGE
        CreateErrorLists();
        _authorDAOMock.Setup(mock => mock.AddAuthor(It.IsAny<Author>())).Returns(false);
        _authorValidatorMock.Setup(author => author.IsValid(It.IsAny<Author>(), out _actualErrors)).Returns(true);


        // ACT
        bool added = _sut.AddAuthor(It.IsAny<Author>(), out _actualErrors);

        //ASSERT
        Assert.IsFalse(added);
    }

    [Test]
    public void RemoveAuthor_Removed()
    {
        // ARRANGE
        Author author = new Author("Alexander", "Pushkin") {Id = 1};
        _authorDAOMock.Setup(mock => mock.RemoveAuthor(1));
        _authorDAOMock.Setup(mock => mock.GetAuthorById(1)).Returns(author);

        // ACT
        bool removed = _sut.RemoveAuthor(1, out _actualErrors);

        //ASSERT
        Assert.IsTrue(removed);
    }

    [Test]
    public void RemoveAuthor_NotRemoved()
    {
        // ARRANGE
        CreateErrorLists();
        _expectedErrors.Add(new Error(ErrorType.Value, ErrorMessages.ErrorMessagePolygraphyAuthorsNotExist));
        Author author = new Author("Alexander", "Pushkin") {Id = 1};
        _authorDAOMock.Setup(mock => mock.RemoveAuthor(2));
        _authorDAOMock.Setup(mock => mock.GetAuthorById(2)).Returns((Author) null);

        // ACT
        bool result = _sut.RemoveAuthor(2, out _actualErrors);

        //ASSERT
        Assert.Multiple(() =>
        {
            Assert.IsTrue(Enumerable.SequenceEqual(_expectedErrors.OrderBy(e => e), _actualErrors.OrderBy(e => e)));
            Assert.IsFalse(result);
        });
    }

    private void CreateErrorLists()
    {
        _expectedErrors = new List<Error>();
        _actualErrors = new List<Error>();
    }
}