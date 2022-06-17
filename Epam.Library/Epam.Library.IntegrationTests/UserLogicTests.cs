using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Epam.Library.BLL;
using Epam.Library.BLL.Interfaces;
using Epam.Library.Entities;
using Epam.Library.DependencyConfig;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Epam.Library.IntegrationTests;

public class UserLogicTests
{
    private IUserLogic _sut;
    private List<Error> _expectedErrors;
    private List<Error> _actualErrors;
    private List<User> _expectedUsers;
    private List<User> _actualUsers;

    [SetUp]
    public void Setup()
    {
        var services = new ServiceCollection();
        _sut = Config.RegisterServices(services).GetService<IUserLogic>();
    }

    [TearDown]
    public void Cleanup()
    {
        _sut.ClearUsers();
    }

    [Test]
    public void Auth_Success()
    {
        // ARRANGE
        var user = CreateUser();

        // ACT
        _sut.Register(user, out _actualErrors);
        var result = _sut.Auth(user, out _actualErrors);

        // ASSERT
        Assert.IsTrue(result);
    }

    [Test]
    public void Auth_Failure_Incorrect_Username()
    {
        // ARRANGE
        CreateErrorsLists();
        User user = CreateUser();
        user.Username = "1a";
        _expectedErrors.Add(new Error(ErrorType.Value, ErrorMessages.ErrorMessageUserIncorrectUsername));

        // ACT
        var result = _sut.Auth(user, out _actualErrors);

        // ASSERT
        Assert.Multiple(() =>
        {
            Assert.IsFalse(result);
            Assert.IsTrue(_expectedErrors.OrderBy(e => e).SequenceEqual(_actualErrors.OrderBy(e => e)));
        });
    }

    [Test]
    public void Auth_Failure_Incorrect_Password()
    {
        // ARRANGE
        CreateErrorsLists();
        User user = CreateUser();
        user.Password = "13";
        _expectedErrors.Add(new Error(ErrorType.Length, ErrorMessages.ErrorMessageUserPasswordShort));

        // ACT
        var result = _sut.Auth(user, out _actualErrors);

        // ASSERT
        Assert.Multiple(() =>
        {
            Assert.IsFalse(result);
            Assert.IsTrue(_expectedErrors.OrderBy(e => e).SequenceEqual(_actualErrors.OrderBy(e => e)));
        });
    }

    [Test]
    public void Register_Success()
    {
        // ARRANGE
        User user = CreateUser();

        // ACT
        var result = _sut.Register(user, out List<Error> errors);

        // ASSERT
        Assert.IsTrue(result);
    }

    [Test]
    public void Register_Failure_Incorrect_Username()
    {
        // ARRANGE
        CreateErrorsLists();
        User user = CreateUser();
        user.Username = "_";
        _expectedErrors.Add(new Error(ErrorType.Value, ErrorMessages.ErrorMessageUserIncorrectUsername));

        // ACT
        var result = _sut.Register(user, out _actualErrors);

        // ASSERT
        Assert.Multiple(() =>
        {
            Assert.IsFalse(result);
            Assert.IsTrue(_expectedErrors.OrderBy(e => e).SequenceEqual(_actualErrors.OrderBy(e => e)));
        });
    }

    [Test]
    public void Register_Failure_Incorrect_Password()
    {
        // ARRANGE
        CreateErrorsLists();
        User user = CreateUser();
        user.Password = "13";
        _expectedErrors.Add(new Error(ErrorType.Length, ErrorMessages.ErrorMessageUserPasswordShort));

        // ACT
        var result = _sut.Register(user, out _actualErrors);

        // ASSERT
        Assert.Multiple(() =>
        {
            Assert.IsFalse(result);
            Assert.IsTrue(_expectedErrors.OrderBy(e => e.Message).SequenceEqual(_actualErrors.OrderBy(e => e.Message)));
        });
    }

    [Test]
    public void GetUserByUsername_Success()
    {
        // ARRANGE
        CreateUsersLists();
        User user = CreateUser();
        _expectedUsers.Add(user);

        // ACT
        _sut.Register(user, out _actualErrors);
        _actualUsers.Add(_sut.GetUserByUsername(user.Username));

        // ASSERT
        Assert.IsTrue(_expectedUsers.OrderBy(u => u.Username).SequenceEqual(_actualUsers.OrderBy(u => u.Username)));
    }

    [Test]
    public void GetUserByUsername_Failure()
    {
        // ARRANGE

        // ACT
        var user = _sut.GetUserByUsername("Nonexistentuser");

        // ASSERT
        Assert.IsNull(user);
    }

    [Test]
    public void UpdateUser_Success()
    {
        // ARRANGE
        var user = CreateUser();

        // ACT
        var result = _sut.UpdateUser(user, out _actualErrors);

        // ASSERT
        Assert.IsTrue(result);
    }

    [Test]
    public void UpdateUser_Failure_DueToUserIsNull()
    {
        // ARRANGE
        User? user = null;

        // ACT

        // ASSERT
        Assert.Throws<ArgumentNullException>(() => _sut.UpdateUser(user, out _actualErrors));
    }

    [Test]
    public void UpdateUser_Failure_DueToUniquenessError()
    {
        // ARRANGE
        CreateErrorsLists();
        User user1 = CreateUser();
        User user2 = CreateUser();
        _expectedErrors.Add(new Error(ErrorType.Value, ErrorMessages.ErrorMessageUserAlreadyExist));

        // ACT
        _sut.Register(user1, out _actualErrors);
        var result = _sut.UpdateUser(user2, out _actualErrors);


        // ASSERT
        Assert.Multiple(() =>
        {
            Assert.IsFalse(result);
            Assert.IsTrue(_expectedErrors.OrderBy(e => e).SequenceEqual(_actualErrors.OrderBy(e => e)));
        });
    }

    [Test]
    public void UpdateUser_Failure_DueToValidationError()
    {
        // ARRANGE
        CreateErrorsLists();
        User user = CreateUser();
        user.Username = "1a";
        _expectedErrors.Add(new Error(ErrorType.Value, ErrorMessages.ErrorMessageUserIncorrectUsername));

        // ACT
        var result = _sut.UpdateUser(user, out _actualErrors);


        // ASSERT
        Assert.Multiple(() =>
        {
            Assert.IsFalse(result);
            Assert.IsTrue(_expectedErrors.OrderBy(e => e.Message).SequenceEqual(_actualErrors.OrderBy(e => e.Message)));
        });
    }

    [Test]
    public void RemoveUser_Removed()
    {
        // ARRANGE
        User user = CreateUser();

        // ACT
        _sut.Register(user, out _actualErrors);
        var result = _sut.RemoveUser(user.Id, out _actualErrors);

        // ASSERT
        Assert.IsTrue(result);
    }

    [Test]
    public void RemoveUser_NotRemoved()
    {
        // ARRANGE
        CreateErrorsLists();
        _expectedErrors.Add(new Error(ErrorType.Value, ErrorMessages.ErrorMessageUserNotExist));

        // ACT
        var result = _sut.RemoveUser(0, out _actualErrors);

        // ASSERT
        Assert.Multiple(() =>
        {
            Assert.IsTrue(_actualErrors.OrderBy(e => e).SequenceEqual(_expectedErrors.OrderBy(e => e)));
            Assert.IsFalse(result);
        });
    }

    [Test]
    public void GetUserById_Exists()
    {
        // ARRANGE
        User user = CreateUser();

        // ACT
        _sut.Register(user, out _actualErrors);
        var result = _sut.GetUserById(user.Id);

        // ASSERT
        Assert.AreEqual(user, result);
    }

    [Test]
    public void GetUserById_NotExists()
    {
        // ARRANGE

        // ACT
        var result = _sut.GetUserById(0);

        // ASSERT
        Assert.IsNull(result);
    }

    private void CreateErrorsLists()
    {
        _expectedErrors = new List<Error>();
        _actualErrors = new List<Error>();
    }

    private void CreateUsersLists()
    {
        _expectedUsers = new List<User>();
        _actualUsers = new List<User>();
    }

    private User CreateUser()
    {
        return new User("ValidUser".ToLower(), GetHashedPassword("1234"));
    }
    
    private string GetHashedPassword(string password)
    {
        using var sha = SHA512.Create();
        var sb = new StringBuilder();
        foreach (var item in sha.ComputeHash(Encoding.Unicode.GetBytes(password)))
        {
            sb.Append(item.ToString("X2"));
        }

        return sb.ToString();
    }
}