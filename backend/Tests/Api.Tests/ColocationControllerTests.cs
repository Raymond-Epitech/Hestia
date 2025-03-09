using Api.Controllers;
using Business.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shared.Exceptions;
using Shared.Models.Input;
using Shared.Models.Output;
using Shared.Models.Update;

namespace Tests.Hestia;

public class ColocationControllerTests
{
    private readonly Mock<IColocationService> _colocationServiceMock;
    private readonly ColocationController _controller;

    public ColocationControllerTests()
    {
        _colocationServiceMock = new Mock<IColocationService>();
        _controller = new ColocationController(_colocationServiceMock.Object);
    }

    // GET ALL COLOCATIONS

    [Fact]
    public async Task GetAllColocations_ReturnsOk_WhenColocationsExist()
    {
        // Arrange
        var colocationList = new List<ColocationOutput>
        {
            new ColocationOutput
            {
                Id = Guid.NewGuid(),
                Name = "Colocation 1",
                Address = "Adresse test",
                Colocataires = null
            },
            new ColocationOutput
            {
                Id = Guid.NewGuid(),
                Name = "Colocation 2",
                Address = "Adresse test",
                Colocataires = null
            }
        };

        _colocationServiceMock.Setup(service => service.GetAllColocations()).ReturnsAsync(colocationList);

        // Act
        var actionResult = await _controller.GetAllCollocations();

        // Assert
        actionResult.Result.Should().BeOfType<OkObjectResult>();

        var okResult = actionResult.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().BeEquivalentTo(colocationList);

        _colocationServiceMock.Verify(service => service.GetAllColocations(), Times.Once);
    }

    [Fact]
    public async Task GetAllColocations_ShouldReturnContextError_WhenInvalid()
    {
        // Arrange
        _colocationServiceMock.Setup(service => service.GetAllColocations())
            .ThrowsAsync(new ContextException("Context error"));

        // Act
        var actionResult = await _controller.GetAllCollocations();

        // Assert
        actionResult.Result.Should().BeOfType<UnprocessableEntityObjectResult>();
        _colocationServiceMock.Verify(service => service.GetAllColocations(), Times.Once);
    }

    // GET COLOCATION

    [Fact]
    public async Task GetColocation_ReturnsOk_WhenColocationExists()
    {
        // Arrange
        var colocationId = Guid.NewGuid();
        var expectedColocation = new ColocationOutput
        {
            Id = Guid.NewGuid(),
            Name = "Colocation 1",
            Address = "Adresse test",
            Colocataires = null
        };

        _colocationServiceMock.Setup(service => service.GetColocation(colocationId)).ReturnsAsync(expectedColocation);

        // Act
        var actionResult = await _controller.GetColocation(colocationId);

        // Assert
        actionResult.Result.Should().BeOfType<OkObjectResult>();

        var okResult = actionResult.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().BeEquivalentTo(expectedColocation);

        _colocationServiceMock.Verify(service => service.GetColocation(colocationId), Times.Once);
    }

    [Fact]
    public async Task GetColocation_ShouldReturnNotFound_WhenColocationDoesNotExist()
    {
        // Arrange
        var colocationId = Guid.NewGuid();

        _colocationServiceMock.Setup(service => service.GetColocation(colocationId))
            .ThrowsAsync(new NotFoundException("Colocation not found"));

        // Act
        var actionResult = await _controller.GetColocation(colocationId);

        // Assert
        actionResult.Result.Should().BeOfType<NotFoundObjectResult>();
        _colocationServiceMock.Verify(service => service.GetColocation(colocationId), Times.Once);
    }

    [Fact]
    public async Task GetColocation_ShouldReturnContextError_WhenInvalid()
    {
        // Arrange
        var colocationId = Guid.NewGuid();

        _colocationServiceMock.Setup(service => service.GetColocation(colocationId))
            .ThrowsAsync(new ContextException("Context error"));

        // Act
        var actionResult = await _controller.GetColocation(colocationId);

        // Assert
        actionResult.Result.Should().BeOfType<UnprocessableEntityObjectResult>();
        _colocationServiceMock.Verify(service => service.GetColocation(colocationId), Times.Once);
    }

    // ADD COLOCATION

    [Fact]
    public async Task AddColocation_ReturnsOk_WhenSuccessful()
    {
        // Arrange
        var colocationInput = new ColocationInput
        {
            Name = "Colocation 1",
            Address = "Adresse test",
            CreatedBy = "testUser"
        };
        var addedBy = Guid.NewGuid();
        var colocationId = Guid.NewGuid();

        _colocationServiceMock.Setup(service => service.AddColocation(colocationInput, addedBy)).ReturnsAsync(colocationId);

        // Act
        var actionResult = await _controller.AddCollocation(colocationInput, addedBy);

        // Assert
        actionResult.Result.Should().BeOfType<OkObjectResult>();

        var okResult = actionResult.Result as OkObjectResult;
        okResult!.Value.Should().Be(colocationId);

        _colocationServiceMock.Verify(service => service.AddColocation(colocationInput, addedBy), Times.Once);
    }

    [Fact]
    public async Task AddColocation_ShouldReturnContextError_WhenInvalid()
    {
        // Arrange
        var colocationInput = new ColocationInput
        {
            Name = "Colocation 1",
            Address = "Adresse test",
            CreatedBy = "testUser"
        };
        var addedBy = Guid.NewGuid();

        _colocationServiceMock.Setup(service => service.AddColocation(colocationInput, addedBy))
            .ThrowsAsync(new ContextException("Invalid colocation"));

        // Act
        var actionResult = await _controller.AddCollocation(colocationInput, addedBy);

        // Assert
        actionResult.Result.Should().BeOfType<UnprocessableEntityObjectResult>();
        _colocationServiceMock.Verify(service => service.AddColocation(colocationInput, addedBy), Times.Once);
    }

    // 🔹 UPDATE COLOCATION

    [Fact]
    public async Task UpdateColocation_ReturnsOk_WhenSuccessful()
    {
        // Arrange
        var colocationUpdate = new ColocationUpdate
        {
            Id = Guid.NewGuid(),
            Name = "Colocation 1",
            Address = "Adresse test"
        };

        _colocationServiceMock.Setup(service => service.UpdateColocation(colocationUpdate));

        // Act
        var actionResult = await _controller.UpdateCollocation(colocationUpdate);

        // Assert
        actionResult.Should().BeOfType<OkResult>();
        _colocationServiceMock.Verify(service => service.UpdateColocation(colocationUpdate), Times.Once);
    }

    [Fact]
    public async Task UpdateColocation_ShouldReturnNotFound_WhenColocationDoesNotExist()
    {
        // Arrange
        var colocationUpdate = new ColocationUpdate
        {
            Id = Guid.NewGuid(),
            Name = "Colocation 1",
            Address = "Adresse test"
        };

        _colocationServiceMock.Setup(service => service.UpdateColocation(colocationUpdate))
            .ThrowsAsync(new NotFoundException("Colocation not found"));

        // Act
        var actionResult = await _controller.UpdateCollocation(colocationUpdate);

        // Assert
        actionResult.Should().BeOfType<NotFoundObjectResult>();
        _colocationServiceMock.Verify(service => service.UpdateColocation(colocationUpdate), Times.Once);
    }

    // 🔹 DELETE COLOCATION

    [Fact]
    public async Task DeleteColocation_ReturnsOk_WhenSuccessful()
    {
        // Arrange
        var colocationId = Guid.NewGuid();

        _colocationServiceMock.Setup(service => service.DeleteColocation(colocationId));

        // Act
        var actionResult = await _controller.DeleteCollocation(colocationId);

        // Assert
        actionResult.Should().BeOfType<OkResult>();
        _colocationServiceMock.Verify(service => service.DeleteColocation(colocationId), Times.Once);
    }

    [Fact]
    public async Task DeleteColocation_ShouldReturnNotFound_WhenColocationDoesNotExist()
    {
        // Arrange
        var colocationId = Guid.NewGuid();

        _colocationServiceMock.Setup(service => service.DeleteColocation(colocationId))
            .ThrowsAsync(new NotFoundException("Colocation not found"));

        // Act
        var actionResult = await _controller.DeleteCollocation(colocationId);

        // Assert
        actionResult.Should().BeOfType<NotFoundObjectResult>();
        _colocationServiceMock.Verify(service => service.DeleteColocation(colocationId), Times.Once);
    }
}
