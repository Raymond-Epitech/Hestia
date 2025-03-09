using Business.Interfaces;
using Business.Services;
using EntityFramework.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Shared.Exceptions;
using Shared.Models.Output;
using Shared.Models.Update;

namespace Tests.Hestia;

public class UserServiceTests
{
    private readonly UserService _userService;
    private readonly Mock<ILogger<UserService>> _loggerMock;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IJwtService> _jwtServMock;

    public UserServiceTests()
    {
        _userRepoMock = new Mock<IUserRepository>();
        _loggerMock = new Mock<ILogger<UserService>>();
        _jwtServMock = new Mock<IJwtService>();

        _userService = new UserService(_loggerMock.Object, _userRepoMock.Object, _jwtServMock.Object);
    }

    // GET ALL USERS

    [Fact]
    public async Task GetAllUserByColocationId_ShouldReturnListOfUser_WhenUsersExists()
    {
        // Arrange
        var colocationId = Guid.NewGuid();

        var expectedUserList = new List<UserOutput>
        {
            new UserOutput
            {
                Id = Guid.NewGuid(),
                Username = "TestUser",
                Email = "test@example.com",
                ColocationId = null
            }
        };

        _userRepoMock.Setup(repo => repo.GetAllUserOutputAsync(colocationId)).ReturnsAsync(expectedUserList);

        // Act
        var result = await _userService.GetAllUser(colocationId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedUserList);
        _userRepoMock.Verify(repo => repo.GetAllUserOutputAsync(colocationId), Times.Once);
    }

    [Fact]
    public async Task GetAllUserByColocationId_ShouldReturnContextError_WhenIsInvalid()
    {
        // Arrange
        var colocationId = Guid.NewGuid();

        _userRepoMock.Setup(repo => repo.GetAllUserOutputAsync(colocationId))
            .ThrowsAsync(new ContextException("Colocation id do not exist"));

        // Act & Assert
        await Assert.ThrowsAsync<ContextException>(() => _userService.GetAllUser(colocationId));

        _userRepoMock.Verify(repo => repo.GetAllUserOutputAsync(colocationId), Times.Once);
    }

    // GET USER

    [Fact]
    public async Task GetUserById_ShouldReturnUser_WhenUserExists()
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
        _userRepoMock.Setup(repo => repo.GetUserOutputByIdAsync(userId)).ReturnsAsync(expectedUser);

        // Act
        var result = await _userService.GetUser(userId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedUser);
    }

    [Fact]
    public async Task GetUserById_ShouldReturnNotFound_WhenUserDoNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _userRepoMock.Setup(repo => repo.GetUserOutputByIdAsync(userId))
            .ThrowsAsync(new NotFoundException("user id do not exist"));

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _userService.GetUser(userId));

        _userRepoMock.Verify(repo => repo.GetUserOutputByIdAsync(userId), Times.Once);
    }

    [Fact]
    public async Task GetUserById_ShouldReturnContextError_WhenUserIsInvalid()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _userRepoMock.Setup(repo => repo.GetUserOutputByIdAsync(userId))
            .ThrowsAsync(new ContextException("user is invalid"));

        // Act & Assert
        await Assert.ThrowsAsync<ContextException>(() => _userService.GetUser(userId));

        _userRepoMock.Verify(repo => repo.GetUserOutputByIdAsync(userId), Times.Once);
    }

    // UPDATE USER

    [Fact]
    public async Task UpdateUser_ShouldUpdateUser_WhenUserExists()
    {
        // Arrange
        var userUpdate = new UserUpdate
        {
            Id = Guid.NewGuid(),
            Username = "UpdatedUser",
            Email = "updated@example.com",
            ColocationId = null
        };

        var existingUser = new User
        {
            Id = userUpdate.Id,
            CreatedAt = DateTime.UtcNow,
            LastConnection = DateTime.UtcNow,
            ColocationId = Guid.NewGuid(),
            Username = "OldUser",
            Email = "old@example.com",
            PathToProfilePicture = "/"
        };

        _userRepoMock.Setup(repo => repo.GetUserByIdAsync(userUpdate.Id)).ReturnsAsync(existingUser);

        User updatedUser = null!;
        _userRepoMock.Setup(repo => repo.UpdateAsync(It.IsAny<User>()))
            .Callback<User>(u => updatedUser = u);

        // Act
        await _userService.UpdateUser(userUpdate);

        // Assert
        _userRepoMock.Verify(repo => repo.GetUserByIdAsync(userUpdate.Id), Times.Once);

        updatedUser.Should().NotBeNull();

        updatedUser.Username.Should().Be(userUpdate.Username);
        updatedUser.PathToProfilePicture.Should().Be(userUpdate.PathToProfilePicture);
        updatedUser.ColocationId.Should().Be(userUpdate.ColocationId);

        updatedUser.CreatedAt.Should().Be(existingUser.CreatedAt);
        updatedUser.Email.Should().Be(existingUser.Email);
    }
}

