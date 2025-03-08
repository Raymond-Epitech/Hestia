using Business.Services;
using FluentAssertions;
using Moq;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _userRepoMock = new Mock<IUserRepository>();
        _userService = new UserService(_userRepoMock.Object);
    }

    [Fact]
    public async Task GetUserById_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var userId = 1;
        var expectedUser = new User { Id = userId, Name = "TestUser" };
        _userRepoMock.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(expectedUser);

        // Act
        var result = await _userService.GetUserById(userId);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("TestUser");
    }

    [Fact]
    public async Task GetUserById_ShouldThrowException_WhenUserNotFound()
    {
        // Arrange
        var userId = 99;
        _userRepoMock.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync((User)null);

        // Act
        Func<Task> act = async () => await _userService.GetUserById(userId);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>().WithMessage("User not found");
    }
}
