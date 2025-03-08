using Api.Controllers;
using Business.Interfaces;
using EntityFramework.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shared.Exceptions;
using Shared.Models.Output;

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
        var userId = Guid.NewGuid();
        var expectedUser = new UserOutput
        {
            Id = userId,
            Username = "TestUser",
            Email = "test@example.com",
            ColocationId = null
        };
        _userServiceMock.Setup(service => service.GetUser(userId)).ReturnsAsync(expectedUser);

        // Act
        var actionResult = await _controller.GetUser(userId);

        // Assert
        actionResult.Result.Should().BeOfType<OkObjectResult>();

        var okResult = actionResult.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().BeEquivalentTo(expectedUser);
    }

    [Fact]
    public async Task GetUserById_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userServiceMock.Setup(service => service.GetUser(userId))
            .ThrowsAsync(new NotFoundException("User not found"));

        // Act
        var actionResult = await _controller.GetUser(userId);

        // Assert
        actionResult.Result.Should().BeOfType<NotFoundObjectResult>();

        var notFoundResult = actionResult.Result as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
    }
}
