using Api.Controllers;
using Business.Interfaces;
using Business.Jwt;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shared.Exceptions;
using Shared.Models.Input;
using Shared.Models.Output;
using Shared.Models.Update;

namespace Tests.Hestia.Controller;

public class UserControllerTests
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly UserController _controller;

    public UserControllerTests()
    {
        _userServiceMock = new Mock<IUserService>();
        _controller = new UserController(_userServiceMock.Object);
    }

    // Get all user

    [Fact]
    public async Task GetAllUser_ReturnsOk_WhenUserListIsNotEmpty()
    {
        // Arange
        var colocationId = Guid.NewGuid();
        var userList = new List<UserOutput>()
        {
            new UserOutput
            {
                Id = Guid.NewGuid(),
                Username = "TestUser1",
                Email = "test@example.com",
                ColocationId = colocationId
            },
            new UserOutput
            {
                Id = Guid.NewGuid(),
                Username = "TestUser2",
                Email = "test@example.com",
                ColocationId = colocationId
            }
        };
        _userServiceMock.Setup(service => service.GetAllUserAsync(colocationId)).ReturnsAsync(userList);

        // Act
        var actionResult = await _controller.GetAllUser(colocationId);

        // Assert
        actionResult.Result.Should().BeOfType<OkObjectResult>();

        var okResult = actionResult.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().BeEquivalentTo(userList);
        _userServiceMock.Verify(service => service.GetAllUserAsync(colocationId), Times.Once);
    }

    [Fact]
    public async Task GetAllUser_ShouldReturnContextError_WhenUserListIsInvalid()
    {
        // Arange
        var colocationId = Guid.NewGuid();

        _userServiceMock.Setup(service => service.GetAllUserAsync(colocationId))
            .ThrowsAsync(new ContextException("Context error"));

        // Act
        var actionResult = await _controller.GetAllUser(colocationId);

        // Assert
        actionResult.Result.Should().BeOfType<UnprocessableEntityObjectResult>();
        _userServiceMock.Verify(service => service.GetAllUserAsync(colocationId), Times.Once);
    }

    // Get User

    [Fact]
    public async Task GetUser_ReturnsOk_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var expectedUser = new UserOutput
        {
            Id = userId,
            Username = "TestUser",
            Email = "test@example.com",
            ColocationId = null
        };
        _userServiceMock.Setup(service => service.GetUserAsync(userId)).ReturnsAsync(expectedUser);

        // Act
        var actionResult = await _controller.GetUser(userId);

        // Assert
        actionResult.Result.Should().BeOfType<OkObjectResult>();

        var okResult = actionResult.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().BeEquivalentTo(expectedUser);
        _userServiceMock.Verify(service => service.GetUserAsync(userId), Times.Once);
    }

    [Fact]
    public async Task GetUser_ShouldReturnNotFound_WhenUserDoNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userServiceMock.Setup(service => service.GetUserAsync(userId))
            .ThrowsAsync(new NotFoundException("User not found"));

        // Act
        var actionResult = await _controller.GetUser(userId);

        // Assert
        actionResult.Result.Should().BeOfType<NotFoundObjectResult>();
        _userServiceMock.Verify(service => service.GetUserAsync(userId), Times.Once);
    }

    [Fact]
    public async Task GetUser_ShouldReturnContextError_WhenUserIsInvalid()
    {
        // Arange
        var userId = Guid.NewGuid();

        _userServiceMock.Setup(service => service.GetUserAsync(userId))
            .ThrowsAsync(new ContextException("Context error"));

        // Act
        var actionResult = await _controller.GetUser(userId);

        // Assert
        actionResult.Result.Should().BeOfType<UnprocessableEntityObjectResult>();
        _userServiceMock.Verify(service => service.GetUserAsync(userId), Times.Once);
    }

    // Update User

    [Fact]
    public async Task UpdateUser_ReturnsOk_WhenUserExists()
    {
        // Arrange
        var expectedUser = new UserUpdate
        {
            Id = Guid.NewGuid(),
            Username = "TestNewUser",
            Email = "test@example.com",
            ColocationId = null
        };
        _userServiceMock.Setup(service => service.UpdateUserAsync(expectedUser));

        // Act
        var actionResult = await _controller.UpdateUser(expectedUser);

        // Assert
        actionResult.Should().BeOfType<OkResult>();
        _userServiceMock.Verify(service => service.UpdateUserAsync(expectedUser), Times.Once);
    }

    [Fact]
    public async Task UpdateUser_ReturnsNotFound_WhenUserDoNotExists()
    {
        // Arrange
        var expectedUser = new UserUpdate
        {
            Id = Guid.NewGuid(),
            Username = "TestNewUser",
            Email = "test@example.com",
            ColocationId = null
        };
        _userServiceMock.Setup(service => service.UpdateUserAsync(expectedUser))
            .ThrowsAsync(new NotFoundException("User not found"));

        // Act
        var actionResult = await _controller.UpdateUser(expectedUser);

        // Assert
        actionResult.Should().BeOfType<NotFoundObjectResult>();
        _userServiceMock.Verify(service => service.UpdateUserAsync(expectedUser), Times.Once);
    }

    [Fact]
    public async Task UpdateUser_ReturnsContextError_WhenUserIsInvalid()
    {
        // Arrange
        var expectedUser = new UserUpdate
        {
            Id = Guid.NewGuid(),
            Username = "TestNewUser",
            Email = "test@example.com",
            ColocationId = null
        };
        _userServiceMock.Setup(service => service.UpdateUserAsync(expectedUser))
            .ThrowsAsync(new ContextException("User is invalid"));

        // Act
        var actionResult = await _controller.UpdateUser(expectedUser);

        // Assert
        actionResult.Should().BeOfType<UnprocessableEntityObjectResult>();
        _userServiceMock.Verify(service => service.UpdateUserAsync(expectedUser), Times.Once);
    }

    // Remove User

    [Fact]
    public async Task RemoveUser_ReturnsOk_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userServiceMock.Setup(service => service.DeleteUserAsync(userId));

        // Act
        var actionResult = await _controller.DeleteUser(userId);

        // Assert
        actionResult.Should().BeOfType<OkResult>();
        _userServiceMock.Verify(service => service.DeleteUserAsync(userId), Times.Once);
    }

    [Fact]
    public async Task RemoveUser_ReturnsNotFound_WhenUserDoesNotExists()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _userServiceMock.Setup(service => service.DeleteUserAsync(userId))
            .ThrowsAsync(new NotFoundException("User not found"));

        // Act
        var actionResult = await _controller.DeleteUser(userId);

        // Assert
        actionResult.Should().BeOfType<NotFoundObjectResult>();
        _userServiceMock.Verify(service => service.DeleteUserAsync(userId), Times.Once);
    }

    [Fact]
    public async Task RemoveUser_ReturnsContextError_WhenUserIsInvalid()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _userServiceMock.Setup(service => service.DeleteUserAsync(userId))
            .ThrowsAsync(new ContextException("User invalid"));

        // Act
        var actionResult = await _controller.DeleteUser(userId);

        // Assert
        actionResult.Should().BeOfType<UnprocessableEntityObjectResult>();
        _userServiceMock.Verify(service => service.DeleteUserAsync(userId), Times.Once);
    }

    // Register

    [Fact]
    public async Task Register_ReturnsOk_WhenTokenIsValid()
    {
        // Arrange
        var googleToken = "ValidGoogleToken";
        var userInput = new UserInput
        {
            Username = "test",
            ColocationId = Guid.NewGuid()
        };
        var userInfo = new UserInfo
        {
            Jwt = "validJwt",
            User = new UserOutput
            {
                Id = Guid.NewGuid(),
                Username = "TestNewUser",
                Email = "test@example.com",
                ColocationId = null
            }
        };

        _userServiceMock.Setup(service => service.RegisterUserAsync(googleToken, userInput)).ReturnsAsync(userInfo);

        // Act
        var actionResult = await _controller.Register(googleToken, userInput);

        // Assert
        actionResult.Result.Should().BeOfType<OkObjectResult>();

        var okResult = actionResult.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().BeEquivalentTo(userInfo);
        _userServiceMock.Verify(service => service.RegisterUserAsync(googleToken, userInput), Times.Once);
    }

    [Fact]
    public async Task RegisterUser_ReturnsAlreadyExist_WhenUserAlreadyExists()
    {
        // Arrange
        var googleToken = "ValidGoogleToken";
        var userInput = new UserInput
        {
            Username = "test",
            ColocationId = Guid.NewGuid()
        };

        _userServiceMock.Setup(service => service.RegisterUserAsync(googleToken, userInput))
            .ThrowsAsync(new AlreadyExistException("User already exist"));

        // Act
        var actionResult = await _controller.Register(googleToken, userInput);

        // Assert
        actionResult.Result.Should().BeOfType<ConflictObjectResult>();
        _userServiceMock.Verify(service => service.RegisterUserAsync(googleToken, userInput), Times.Once);
    }

    [Fact]
    public async Task RegisterUser_ReturnsContextException_WhenUserIsInvalid()
    {
        // Arrange
        var googleToken = "ValidGoogleToken";
        var userInput = new UserInput
        {
            Username = "test",
            ColocationId = Guid.NewGuid()
        };

        _userServiceMock.Setup(service => service.RegisterUserAsync(googleToken, userInput))
            .ThrowsAsync(new ContextException("User is invalid"));

        // Act
        var actionResult = await _controller.Register(googleToken, userInput);

        // Assert
        actionResult.Result.Should().BeOfType<UnprocessableEntityObjectResult>();
        _userServiceMock.Verify(service => service.RegisterUserAsync(googleToken, userInput), Times.Once);
    }

    // Login

    [Fact]
    public async Task Login_ReturnsOk_WhenTokenIsValidAndUserExist()
    {
        // Arrange
        var googleToken = "ValidGoogleToken";
        var userInfo = new UserInfo
        {
            Jwt = "validJwt",
            User = new UserOutput
            {
                Id = Guid.NewGuid(),
                Username = "TestNewUser",
                Email = "test@example.com",
                ColocationId = null
            }
        };

        _userServiceMock.Setup(service => service.LoginUserAsync(googleToken)).ReturnsAsync(userInfo);

        // Act
        var actionResult = await _controller.Login(googleToken);

        // Assert
        actionResult.Result.Should().BeOfType<OkObjectResult>();

        var okResult = actionResult.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().BeEquivalentTo(userInfo);
        _userServiceMock.Verify(service => service.LoginUserAsync(googleToken), Times.Once);
    }

    [Fact]
    public async Task LoginUser_ReturnsNotFound_WhenUserDoNotExists()
    {
        // Arrange
        var googleToken = "ValidGoogleToken";
        var userInput = new UserInput
        {
            Username = "test",
            ColocationId = Guid.NewGuid()
        };

        _userServiceMock.Setup(service => service.LoginUserAsync(googleToken))
            .ThrowsAsync(new NotFoundException("User not found"));

        // Act
        var actionResult = await _controller.Login(googleToken);

        // Assert
        actionResult.Result.Should().BeOfType<NotFoundObjectResult>();
        _userServiceMock.Verify(service => service.LoginUserAsync(googleToken), Times.Once);
    }
}

