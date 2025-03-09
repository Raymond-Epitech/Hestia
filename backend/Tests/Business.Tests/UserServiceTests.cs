using Business.Interfaces;
using Business.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Shared.Models.Output;

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
}
