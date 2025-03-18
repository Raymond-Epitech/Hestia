using Business.Interfaces;
using Business.Services;
using EntityFramework.Models;
using FluentAssertions;
using Google.Apis.Auth;
using Microsoft.Extensions.Logging;
using Moq;
using Shared.Exceptions;
using Shared.Models.Input;
using Shared.Models.Output;
using Shared.Models.Update;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Tests.Hestia.Service;
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
        var result = await _userService.GetAllUserAsync(colocationId);

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

        _userRepoMock.Setup(repo => repo.GetAllUserOutputAsync(colocationId)).ReturnsAsync(new List<UserOutput>());

        // Act
        var result = await _userService.GetAllUserAsync(colocationId);

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(0);
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
        var result = await _userService.GetUserAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedUser);
    }

    [Fact]
    public async Task GetUserById_ShouldReturnNotFound_WhenUserDoNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _userRepoMock.Setup(repo => repo.GetUserOutputByIdAsync(userId)).ReturnsAsync((UserOutput ?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _userService.GetUserAsync(userId));

        _userRepoMock.Verify(repo => repo.GetUserOutputByIdAsync(userId), Times.Once);
    }

    [Fact]
    public async Task GetUserById_ShouldReturnContextError_WhenUserIsInvalid()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _userRepoMock.Setup(repo => repo.GetUserOutputByIdAsync(userId))
            .ThrowsAsync(new Exception("user is invalid"));

        // Act & Assert
        await Assert.ThrowsAsync<ContextException>(() => _userService.GetUserAsync(userId));

        _userRepoMock.Verify(repo => repo.GetUserOutputByIdAsync(userId), Times.Once);
    }

    // UPDATE USER

    [Fact]
    public async Task UpdateUser_ShouldUpdateUser_WhenUserExist()
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
        _userRepoMock.Setup(repo => repo.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        await _userService.UpdateUserAsync(userUpdate);

        // Assert
        _userRepoMock.Verify(repo => repo.GetUserByIdAsync(userUpdate.Id), Times.Once);
        _userRepoMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);

        existingUser.Username.Should().Be(userUpdate.Username);
        existingUser.PathToProfilePicture.Should().Be(userUpdate.PathToProfilePicture);
        existingUser.ColocationId.Should().Be(userUpdate.ColocationId);

        existingUser.CreatedAt.Should().Be(existingUser.CreatedAt);
        existingUser.Email.Should().Be(existingUser.Email);
    }

    [Fact]
    public async Task UpdateUser_ShouldThrowNotFound_WhenUserDoNotExist()
    {
        // Arrange
        var userUpdate = new UserUpdate
        {
            Id = Guid.NewGuid(),
            Username = "UpdatedUser",
            Email = "updated@example.com",
            ColocationId = null
        };

        _userRepoMock.Setup(repo => repo.GetUserByIdAsync(userUpdate.Id)).ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _userService.UpdateUserAsync(userUpdate));

        _userRepoMock.Verify(repo => repo.GetUserByIdAsync(userUpdate.Id), Times.Once);
        _userRepoMock.Verify(repo => repo.SaveChangesAsync(), Times.Never);
    }

    // REMOVE USER

    [Fact]
    public async Task DeleteUser_ShouldRemoveUser_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var existingUser = new User
        {
            Id = userId,
            CreatedAt = DateTime.UtcNow,
            LastConnection = DateTime.UtcNow,
            Username = "TestUser",
            Email = "test@example.com"
        };

        _userRepoMock.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync(existingUser);
        _userRepoMock.Setup(repo => repo.RemoveAsync(existingUser)).Returns(Task.CompletedTask);
        _userRepoMock.Setup(repo => repo.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        await _userService.DeleteUserAsync(userId);

        // Assert
        _userRepoMock.Verify(repo => repo.GetUserByIdAsync(userId), Times.Once);
        _userRepoMock.Verify(repo => repo.RemoveAsync(It.Is<User>(u => u.Id == userId)), Times.Once);
        _userRepoMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteUser_ShouldThrowNotFound_WhenUserDoNotExist()
    {
        // Arrange
        var existingUser = new User
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            LastConnection = DateTime.UtcNow,
            Username = "TestUser",
            Email = "test@example.com"
        };

        _userRepoMock.Setup(repo => repo.GetUserByIdAsync(existingUser.Id)).ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _userService.DeleteUserAsync(existingUser.Id));

        _userRepoMock.Verify(repo => repo.GetUserByIdAsync(existingUser.Id), Times.Once);
        _userRepoMock.Verify(repo => repo.SaveChangesAsync(), Times.Never);
    }

    // REGISTER USER

    [Fact]
    public async Task RegisterUser_ShouldRegister_WhenGoogleTokenIsValid()
    {
        // Arrange
        var googleToken = "ValidGoogleToken";

        var validPayload = new GoogleJsonWebSignature.Payload
        {
            Email = "ValidEmail@net.com",
            Name = "Test User",
            Subject = "123456789",
            Picture = "default.jpg"
        };

        var userInput = new UserInput
        {
            Username = "test",
            ColocationId = Guid.NewGuid()
        };

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, validPayload.Subject),
            new Claim(JwtRegisteredClaimNames.Email, validPayload.Email),
            new Claim(JwtRegisteredClaimNames.Name, validPayload.Name),
            new Claim("picture", validPayload.Picture ?? "")
        };

        var jwt = "ValidJwt";

        _jwtServMock.Setup(serv => serv.ValidateGoogleTokenAsync(googleToken)).ReturnsAsync(validPayload);
        _userRepoMock.Setup(repo => repo.AnyExistingUserByEmail(validPayload.Email)).ReturnsAsync(false);
        _jwtServMock.Setup(serv => serv.GenerateToken(It.IsAny<IEnumerable<Claim>>())).Returns(jwt);
        _userRepoMock.Setup(repo => repo.SaveChangesAsync()).Returns(Task.CompletedTask);

        User capturedUser = null!;
        _userRepoMock.Setup(repo => repo.AddAsync(It.IsAny<User>()))
            .Callback<User>(u => capturedUser = u)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _userService.RegisterUserAsync(googleToken, userInput);

        // Assert
        result.Jwt.Should().BeEquivalentTo(jwt);

        capturedUser.Should().NotBeNull();
        capturedUser.Username.Should().Be(userInput.Username);
        capturedUser.Email.Should().Be(validPayload.Email);
        capturedUser.ColocationId.Should().Be(userInput.ColocationId);
        capturedUser.PathToProfilePicture.Should().Be("default.jpg");

        result.User.Should().BeEquivalentTo(new UserOutput
        {
            Id = capturedUser.Id,
            Username = capturedUser.Username,
            Email = capturedUser.Email,
            ColocationId = capturedUser.ColocationId
        });

        _jwtServMock.Verify(repo => repo.ValidateGoogleTokenAsync(googleToken), Times.Once);
        _userRepoMock.Verify(repo => repo.AnyExistingUserByEmail(validPayload.Email), Times.Once);
        _userRepoMock.Verify(repo => repo.AddAsync(It.IsAny<User>()), Times.Once);
        _userRepoMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        _jwtServMock.Verify(serv => serv.GenerateToken(It.IsAny<IEnumerable<Claim>>()), Times.Once);
    }

    [Fact]
    public async Task RegisterUser_ShouldReturnAlreadyExist_WhenGoogleTokenIsValid()
    {
        // Arrange
        var googleToken = "ValidGoogleToken";

        var validPayload = new GoogleJsonWebSignature.Payload
        {
            Email = "AlreadyExistingEmail@net.com",
            Name = "AlreadyExistingUser",
            Subject = "123456789",
            Picture = "default.jpg"
        };

        var userInput = new UserInput
        {
            Username = "test",
            ColocationId = Guid.NewGuid()
        };

        var jwt = "ValidJwt";

        _jwtServMock.Setup(serv => serv.ValidateGoogleTokenAsync(googleToken)).ReturnsAsync(validPayload);
        _userRepoMock.Setup(repo => repo.AnyExistingUserByEmail(validPayload.Email)).ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<AlreadyExistException>(() => _userService.RegisterUserAsync(googleToken, userInput));

        _jwtServMock.Verify(repo => repo.ValidateGoogleTokenAsync(googleToken), Times.Once);
        _userRepoMock.Verify(repo => repo.AnyExistingUserByEmail(validPayload.Email), Times.Once);
        _userRepoMock.Verify(repo => repo.AddAsync(It.IsAny<User>()), Times.Never);
        _userRepoMock.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        _jwtServMock.Verify(serv => serv.GenerateToken(It.IsAny<IEnumerable<Claim>>()), Times.Never);
    }

    [Fact]
    public async Task RegisterUser_ShouldThrowInvalidToken_WhenGoogleTokenIsInvalid()
    {
        // Arrange
        var googleToken = "InvalidGoogleToken";

        var validPayload = new GoogleJsonWebSignature.Payload
        {
            Email = "ValidEmail@net.com",
            Name = "Test User",
            Subject = "123456789",
            Picture = "default.jpg"
        };

        var userInput = new UserInput
        {
            Username = "test",
            ColocationId = Guid.NewGuid()
        };

        _jwtServMock.Setup(serv => serv.ValidateGoogleTokenAsync(googleToken))
            .ThrowsAsync(new Exception("Invalid token"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidTokenException>(() => _userService.RegisterUserAsync(googleToken, userInput));

        _jwtServMock.Verify(repo => repo.ValidateGoogleTokenAsync(googleToken), Times.Once);
        _userRepoMock.Verify(repo => repo.AnyExistingUserByEmail(validPayload.Email), Times.Never);
        _userRepoMock.Verify(repo => repo.AddAsync(It.IsAny<User>()), Times.Never);
        _userRepoMock.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        _jwtServMock.Verify(serv => serv.GenerateToken(It.IsAny<IEnumerable<Claim>>()), Times.Never);
    }

    // LOGIN USER

    [Fact]
    public async Task LoginUser_ShouldLogin_WhenGoogleTokenIsValid()
    {
        // Arrange
        var googleToken = "ValidGoogleToken";

        var validPayload = new GoogleJsonWebSignature.Payload
        {
            Email = "ValidEmail@net.com",
            Name = "Test User",
            Subject = "123456789",
            Picture = "default.jpg"
        };

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, validPayload.Subject),
            new Claim(JwtRegisteredClaimNames.Email, validPayload.Email),
            new Claim(JwtRegisteredClaimNames.Name, validPayload.Name),
            new Claim("picture", validPayload.Picture ?? "")
        };

        var user = new User
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            LastConnection = DateTime.UtcNow,
            Username = "TestUser",
            Email = "test@example.com"
        };

        var jwt = "ValidJwt";

        _jwtServMock.Setup(serv => serv.ValidateGoogleTokenAsync(googleToken)).ReturnsAsync(validPayload);
        _userRepoMock.Setup(repo => repo.GetUserByEmailAsync(validPayload.Email)).ReturnsAsync(user);
        _userRepoMock.Setup(repo => repo.SaveChangesAsync()).Returns(Task.CompletedTask);
        _jwtServMock.Setup(serv => serv.GenerateToken(It.IsAny<IEnumerable<Claim>>())).Returns(jwt);

        // Act
        var result = await _userService.LoginUserAsync(googleToken);

        // Assert
        result.Jwt.Should().BeEquivalentTo(jwt);

        _jwtServMock.Verify(repo => repo.ValidateGoogleTokenAsync(googleToken), Times.Once);
        _userRepoMock.Verify(repo => repo.GetUserByEmailAsync(validPayload.Email), Times.Once);
        _userRepoMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        _jwtServMock.Verify(serv => serv.GenerateToken(It.IsAny<IEnumerable<Claim>>()), Times.Once);
    }

    [Fact]
    public async Task LoginUser_ShouldReturnUserNotFound_WhenUserDoNotExist()
    {
        // Arrange
        var googleToken = "ValidGoogleToken";

        var validPayload = new GoogleJsonWebSignature.Payload
        {
            Email = "AlreadyExistingEmail@net.com",
            Name = "AlreadyExistingUser",
            Subject = "123456789",
            Picture = "default.jpg"
        };

        _jwtServMock.Setup(serv => serv.ValidateGoogleTokenAsync(googleToken)).ReturnsAsync(validPayload);
        var result = _userRepoMock.Setup(repo => repo.GetUserByEmailAsync(validPayload.Email)).ReturnsAsync((User ?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _userService.LoginUserAsync(googleToken));

        _jwtServMock.Verify(repo => repo.ValidateGoogleTokenAsync(googleToken), Times.Once);
        _userRepoMock.Verify(repo => repo.GetUserByEmailAsync(validPayload.Email), Times.Once);
        _userRepoMock.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        _jwtServMock.Verify(serv => serv.GenerateToken(It.IsAny<IEnumerable<Claim>>()), Times.Never);
    }

    [Fact]
    public async Task LoginUser_ShouldThrowInvalidToken_WhenTokenIsInvalid()
    {
        // Arrange
        var googleToken = "InvalidGoogleToken";

        var validPayload = new GoogleJsonWebSignature.Payload
        {
            Email = "ValidEmail@net.com",
            Name = "Test User",
            Subject = "123456789",
            Picture = "default.jpg"
        };

        _jwtServMock.Setup(serv => serv.ValidateGoogleTokenAsync(googleToken))
            .ThrowsAsync(new Exception("Invalid token"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidTokenException>(() => _userService.LoginUserAsync(googleToken));

        _jwtServMock.Verify(repo => repo.ValidateGoogleTokenAsync(googleToken), Times.Once);
        _userRepoMock.Verify(repo => repo.GetUserByEmailAsync(validPayload.Email), Times.Never);
        _userRepoMock.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        _jwtServMock.Verify(serv => serv.GenerateToken(It.IsAny<IEnumerable<Claim>>()), Times.Never);
    }
}

