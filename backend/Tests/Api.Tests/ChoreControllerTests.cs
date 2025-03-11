using Api.Controllers;
using Business.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shared.Exceptions;
using Shared.Models.Output;
using Shared.Models.Update;

namespace Tests.Hestia.Controller;
public class ChoreControllerTests
{
    private readonly Mock<IChoreService> _choreServiceMock;
    private readonly ChoreController _controller;

    public ChoreControllerTests()
    {
        _choreServiceMock = new Mock<IChoreService>();
        _controller = new ChoreController(_choreServiceMock.Object);
    }

    // GET ALL CHORES BY COLOCATION ID

    [Fact]
    public async Task GetAllChores_ReturnsOk_WhenChoresExist()
    {
        var colocationId = Guid.NewGuid();
        var chores = new List<ChoreOutput>
        {
            new ChoreOutput
            {
                Id = Guid.NewGuid(),
                Title = "Chore 1",
                IsDone = false
            }
        };

        _choreServiceMock.Setup(service => service.GetAllChoresAsync(colocationId)).ReturnsAsync(chores);

        var actionResult = await _controller.GetAllChores(colocationId);

        actionResult.Result.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeEquivalentTo(chores);
        _choreServiceMock.Verify(service => service.GetAllChoresAsync(colocationId), Times.Once);
    }

    [Fact]
    public async Task GetAllChores_ShouldReturnContextError_WhenInvalid()
    {
        var colocationId = Guid.NewGuid();

        _choreServiceMock.Setup(service => service.GetAllChoresAsync(colocationId))
            .ThrowsAsync(new ContextException("Context error"));

        var actionResult = await _controller.GetAllChores(colocationId);

        actionResult.Result.Should().BeOfType<UnprocessableEntityObjectResult>();
        _choreServiceMock.Verify(service => service.GetAllChoresAsync(colocationId), Times.Once);
    }

    // GET SINGLE CHORE BY ID

    [Fact]
    public async Task GetChore_ReturnsOk_WhenChoreExists()
    {
        var choreId = Guid.NewGuid();

        var chore = new ChoreOutput
        {
            Id = choreId,
            Title = "Test Chore",
            IsDone = false
        };

        _choreServiceMock.Setup(service => service.GetChoreAsync(choreId)).ReturnsAsync(chore);

        var actionResult = await _controller.GetChore(choreId);

        actionResult.Result.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeEquivalentTo(chore);
        _choreServiceMock.Verify(service => service.GetChoreAsync(choreId), Times.Once);
    }

    [Fact]
    public async Task GetChore_ShouldReturnNotFound_WhenChoreDoesNotExist()
    {
        var choreId = Guid.NewGuid();
        _choreServiceMock.Setup(service => service.GetChoreAsync(choreId))
            .ThrowsAsync(new NotFoundException("Chore not found"));

        var actionResult = await _controller.GetChore(choreId);

        actionResult.Result.Should().BeOfType<NotFoundObjectResult>();
        _choreServiceMock.Verify(service => service.GetChoreAsync(choreId), Times.Once);
    }

    [Fact]
    public async Task GetChore_ShouldReturnContextError_WhenInvalid()
    {
        var choreId = Guid.NewGuid();
        _choreServiceMock.Setup(service => service.GetChoreAsync(choreId))
            .ThrowsAsync(new ContextException("Context error"));

        var actionResult = await _controller.GetChore(choreId);

        actionResult.Result.Should().BeOfType<UnprocessableEntityObjectResult>();
        _choreServiceMock.Verify(service => service.GetChoreAsync(choreId), Times.Once);
    }

    // ADD CHORE
    [Fact]
    public async Task AddChore_ReturnsOk_WhenSuccessful()
    {
        var choreInput = new ChoreInput
        {
            Title = "New Chore",
            ColocationId = Guid.NewGuid(),
            IsDone = false
        };

        var choreId = Guid.NewGuid();

        _choreServiceMock.Setup(service => service.AddChoreAsync(choreInput)).ReturnsAsync(choreId);

        var actionResult = await _controller.AddChore(choreInput);

        actionResult.Result.Should().BeOfType<OkObjectResult>().Which.Value.Should().Be(choreId);
        _choreServiceMock.Verify(service => service.AddChoreAsync(choreInput), Times.Once);
    }

    // UPDATE CHORE
    [Fact]
    public async Task UpdateChore_ReturnsOk_WhenSuccessful()
    {
        var choreUpdate = new ChoreUpdate
        {
            Id = Guid.NewGuid(),
            Title = "Updated Chore",
            IsDone = true
        };

        _choreServiceMock.Setup(service => service.UpdateChoreAsync(choreUpdate));

        var actionResult = await _controller.UpdateChore(choreUpdate);

        actionResult.Should().BeOfType<OkResult>();
        _choreServiceMock.Verify(service => service.UpdateChoreAsync(choreUpdate), Times.Once);
    }

    [Fact]
    public async Task UpdateChore_ShouldReturnNotFound_WhenChoreDoesNotExist()
    {
        var choreUpdate = new ChoreUpdate
        {
            Id = Guid.NewGuid(),
            Title = "Updated Chore",
            IsDone = true
        };

        _choreServiceMock.Setup(service => service.UpdateChoreAsync(choreUpdate))
            .ThrowsAsync(new NotFoundException("Chore not found"));

        var actionResult = await _controller.UpdateChore(choreUpdate);

        actionResult.Should().BeOfType<NotFoundObjectResult>();
        _choreServiceMock.Verify(service => service.UpdateChoreAsync(choreUpdate), Times.Once);
    }

    [Fact]
    public async Task UpdateChore_ShouldReturnContextError_WhenInvalid()
    {
        var choreUpdate = new ChoreUpdate
        {
            Id = Guid.NewGuid(),
            Title = "Updated Chore",
            IsDone = true
        };

        _choreServiceMock.Setup(service => service.UpdateChoreAsync(choreUpdate))
            .ThrowsAsync(new ContextException("Invalid chore"));

        var actionResult = await _controller.UpdateChore(choreUpdate);

        actionResult.Should().BeOfType<UnprocessableEntityObjectResult>();
        _choreServiceMock.Verify(service => service.UpdateChoreAsync(choreUpdate), Times.Once);
    }

    // DELETE CHORE
    [Fact]
    public async Task DeleteChore_ReturnsOk_WhenSuccessful()
    {
        var choreId = Guid.NewGuid();
        _choreServiceMock.Setup(service => service.DeleteChoreAsync(choreId));

        var actionResult = await _controller.DeleteChore(choreId);

        actionResult.Should().BeOfType<OkResult>();
        _choreServiceMock.Verify(service => service.DeleteChoreAsync(choreId), Times.Once);
    }

    [Fact]
    public async Task DeleteChore_ShouldReturnNotFound_WhenChoreDoesNotExist()
    {
        var choreId = Guid.NewGuid();
        _choreServiceMock.Setup(service => service.DeleteChoreAsync(choreId))
            .ThrowsAsync(new NotFoundException("Chore not found"));

        var actionResult = await _controller.DeleteChore(choreId);

        actionResult.Should().BeOfType<NotFoundObjectResult>();
        _choreServiceMock.Verify(service => service.DeleteChoreAsync(choreId), Times.Once);
    }

    [Fact]
    public async Task DeleteChore_ShouldReturnContextError_WhenInvalid()
    {
        var choreId = Guid.NewGuid();
        _choreServiceMock.Setup(service => service.DeleteChoreAsync(choreId))
            .ThrowsAsync(new ContextException("Context error"));

        var actionResult = await _controller.DeleteChore(choreId);

        actionResult.Should().BeOfType<UnprocessableEntityObjectResult>();
        _choreServiceMock.Verify(service => service.DeleteChoreAsync(choreId), Times.Once);
    }

    // ENROLL USER TO CHORE
    [Fact]
    public async Task EnrollToChore_ReturnsOk_WhenSuccessful()
    {
        var userId = Guid.NewGuid();
        var choreId = Guid.NewGuid();

        _choreServiceMock.Setup(service => service.EnrollToChore(userId, choreId));

        var actionResult = await _controller.EnrollToChore(userId, choreId);

        actionResult.Should().BeOfType<OkResult>();
        _choreServiceMock.Verify(service => service.EnrollToChore(userId, choreId), Times.Once);
    }

    [Fact]
    public async Task EnrollToChore_ShouldReturnNotFound_WhenUserOrChoreDoesNotExist()
    {
        var userId = Guid.NewGuid();
        var choreId = Guid.NewGuid();

        _choreServiceMock.Setup(service => service.EnrollToChore(userId, choreId))
            .ThrowsAsync(new NotFoundException("Not found"));

        var actionResult = await _controller.EnrollToChore(userId, choreId);

        actionResult.Should().BeOfType<NotFoundObjectResult>();
        _choreServiceMock.Verify(service => service.EnrollToChore(userId, choreId), Times.Once);
    }

    [Fact]
    public async Task EnrollToChore_ShouldReturnContextError_WhenInvalid()
    {
        var userId = Guid.NewGuid();
        var choreId = Guid.NewGuid();

        _choreServiceMock.Setup(service => service.EnrollToChore(userId, choreId))
            .ThrowsAsync(new ContextException("Invalid"));

        var actionResult = await _controller.EnrollToChore(userId, choreId);

        actionResult.Should().BeOfType<UnprocessableEntityObjectResult>();
        _choreServiceMock.Verify(service => service.EnrollToChore(userId, choreId), Times.Once);
    }
}
