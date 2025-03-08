using Api.Controllers;
using Business.Interfaces;
using EntityFramework.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

public class UserControllerTests
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly UserController _controller;

    public UserControllerTests()
    {
        _userServiceMock = new Mock<IUserService>();
        _controller = new UserController(_userServiceMock.Object);
    }

    [Fact]
    public async Task GetUser_ReturnsOk_WhenUserExists()
    {
        // Arrange
        var userId = 1;
        var expectedUser = new User { Id = userId, Name = "TestUser" };
        _userServiceMock.Setup(s => s.GetUserById(userId)).ReturnsAsync(expectedUser);

        // Act
        var result = await _controller.GetUser(userId) as OkObjectResult;

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);
        var user = result.Value as User;
        user.Name.Should().Be("TestUser");
    }
}
