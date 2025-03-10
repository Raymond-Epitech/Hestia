using Api.Controllers;
using Business.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shared.Exceptions;
using Shared.Models.Input;
using Shared.Models.Output;
using Shared.Models.Update;

namespace Tests.Hestia.Controller;

public class ReminderControllerTests
{
    private readonly Mock<IReminderService> _reminderServiceMock;
    private readonly ReminderController _controller;

    public ReminderControllerTests()
    {
        _reminderServiceMock = new Mock<IReminderService>();
        _controller = new ReminderController(_reminderServiceMock.Object);
    }

    // GET ALL REMINDERS BY COLOCATION ID

    [Fact]
    public async Task GetAllReminders_ReturnsOk_WhenRemindersExist()
    {
        var colocationId = Guid.NewGuid();
        var reminders = new List<ReminderOutput>
        {
            new ReminderOutput
            {
                Id = Guid.NewGuid(),
                Content = "Reminder 1"
            }
        };

        _reminderServiceMock.Setup(service => service.GetAllRemindersAsync(colocationId)).ReturnsAsync(reminders);

        var actionResult = await _controller.GetAllReminders(colocationId);

        actionResult.Result.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeEquivalentTo(reminders);
        _reminderServiceMock.Verify(service => service.GetAllRemindersAsync(colocationId), Times.Once);
    }

    [Fact]
    public async Task GetAllReminders_ShouldReturnContextError_WhenInvalid()
    {
        var colocationId = Guid.NewGuid();
        _reminderServiceMock.Setup(service => service.GetAllRemindersAsync(colocationId))
            .ThrowsAsync(new ContextException("Context error"));

        var actionResult = await _controller.GetAllReminders(colocationId);

        actionResult.Result.Should().BeOfType<UnprocessableEntityObjectResult>();
        _reminderServiceMock.Verify(service => service.GetAllRemindersAsync(colocationId), Times.Once);
    }

    // GET SINGLE REMINDER BY ID
    
    [Fact]
    public async Task GetReminder_ReturnsOk_WhenReminderExists()
    {
        var reminderId = Guid.NewGuid();
        var reminder = new ReminderOutput
        {
            Id = reminderId,
            Content = "Test Reminder"
        };

        _reminderServiceMock.Setup(service => service.GetReminderAsync(reminderId)).ReturnsAsync(reminder);

        var actionResult = await _controller.GetReminder(reminderId);

        actionResult.Result.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeEquivalentTo(reminder);
        _reminderServiceMock.Verify(service => service.GetReminderAsync(reminderId), Times.Once);
    }

    [Fact]
    public async Task GetReminder_ShouldReturnNotFound_WhenReminderDoesNotExist()
    {
        var reminderId = Guid.NewGuid();
        _reminderServiceMock.Setup(service => service.GetReminderAsync(reminderId))
            .ThrowsAsync(new NotFoundException("Reminder not found"));

        var actionResult = await _controller.GetReminder(reminderId);

        actionResult.Result.Should().BeOfType<NotFoundObjectResult>();
        _reminderServiceMock.Verify(service => service.GetReminderAsync(reminderId), Times.Once);
    }

    [Fact]
    public async Task GetReminder_ShouldReturnContextError_WhenInvalid()
    {
        var reminderId = Guid.NewGuid();
        _reminderServiceMock.Setup(service => service.GetReminderAsync(reminderId))
            .ThrowsAsync(new ContextException("Context error"));

        var actionResult = await _controller.GetReminder(reminderId);

        actionResult.Result.Should().BeOfType<UnprocessableEntityObjectResult>();
        _reminderServiceMock.Verify(service => service.GetReminderAsync(reminderId), Times.Once);
    }

    // ADD REMINDER

    [Fact]
    public async Task AddReminder_ReturnsOk_WhenSuccessful()
    {
        var reminderInput = new ReminderInput
        {
            Content = "New Reminder",
            ColocationId = Guid.NewGuid()
        };
        var reminderId = Guid.NewGuid();

        _reminderServiceMock.Setup(service => service.AddReminderAsync(reminderInput)).ReturnsAsync(reminderId);

        var actionResult = await _controller.AddReminder(reminderInput);

        actionResult.Result.Should().BeOfType<OkObjectResult>().Which.Value.Should().Be(reminderId);
        _reminderServiceMock.Verify(service => service.AddReminderAsync(reminderInput), Times.Once);
    }

    // UPDATE REMINDER

    [Fact]
    public async Task UpdateReminder_ReturnsOk_WhenSuccessful()
    {
        var reminderUpdate = new ReminderUpdate
        {
            Id = Guid.NewGuid(),
            Content = "Updated Reminder"
        };

        _reminderServiceMock.Setup(service => service.UpdateReminderAsync(reminderUpdate));

        var actionResult = await _controller.UpdateReminder(reminderUpdate);

        actionResult.Should().BeOfType<OkResult>();
        _reminderServiceMock.Verify(service => service.UpdateReminderAsync(reminderUpdate), Times.Once);
    }

    [Fact]
    public async Task UpdateReminder_ShouldReturnNotFound_WhenReminderDoesNotExist()
    {
        var reminderUpdate = new ReminderUpdate
        {
            Id = Guid.NewGuid(),
            Content = "Updated Reminder"
        };

        _reminderServiceMock.Setup(service => service.UpdateReminderAsync(reminderUpdate))
            .ThrowsAsync(new NotFoundException("Reminder not found"));

        var actionResult = await _controller.UpdateReminder(reminderUpdate);

        actionResult.Should().BeOfType<NotFoundObjectResult>();
        _reminderServiceMock.Verify(service => service.UpdateReminderAsync(reminderUpdate), Times.Once);
    }

    [Fact]
    public async Task UpdateReminder_ShouldReturnBadRequest_WhenArgumentMissing()
    {
        var reminderUpdate = new ReminderUpdate
        {
            Id = Guid.NewGuid(),
            Content = "Updated Reminder"
        };

        _reminderServiceMock.Setup(service => service.UpdateReminderAsync(reminderUpdate))
            .ThrowsAsync(new MissingArgumentException("Missing argument"));

        var actionResult = await _controller.UpdateReminder(reminderUpdate);

        actionResult.Should().BeOfType<BadRequestObjectResult>();
        _reminderServiceMock.Verify(service => service.UpdateReminderAsync(reminderUpdate), Times.Once);
    }

    [Fact]
    public async Task UpdateReminder_ShouldReturnContextError_WhenInvalid()
    {
        var reminderUpdate = new ReminderUpdate
        {
            Id = Guid.NewGuid(),
            Content = "Updated Reminder"
        };

        _reminderServiceMock.Setup(service => service.UpdateReminderAsync(reminderUpdate))
            .ThrowsAsync(new ContextException("Invalid reminder"));

        var actionResult = await _controller.UpdateReminder(reminderUpdate);

        actionResult.Should().BeOfType<UnprocessableEntityObjectResult>();
        _reminderServiceMock.Verify(service => service.UpdateReminderAsync(reminderUpdate), Times.Once);
    }

    // UPDATE RANGE REMINDERS

    [Fact]
    public async Task UpdateRangeReminder_ReturnsOk_WhenSuccessful()
    {
        var remindersToUpdate = new List<ReminderUpdate>
        {
            new ReminderUpdate
            {
                Id = Guid.NewGuid(),
                Content = "Updated Reminder"
            }
        };

        _reminderServiceMock.Setup(service => service.UpdateRangeReminderAsync(remindersToUpdate));

        var actionResult = await _controller.UpdateRangeReminder(remindersToUpdate);

        actionResult.Should().BeOfType<OkResult>();
        _reminderServiceMock.Verify(service => service.UpdateRangeReminderAsync(remindersToUpdate), Times.Once);
    }

    // DELETE REMINDER

    [Fact]
    public async Task DeleteReminder_ReturnsOk_WhenSuccessful()
    {
        var reminderId = Guid.NewGuid();
        _reminderServiceMock.Setup(service => service.DeleteReminderAsync(reminderId));

        var actionResult = await _controller.DeleteReminder(reminderId);

        actionResult.Should().BeOfType<OkResult>();
        _reminderServiceMock.Verify(service => service.DeleteReminderAsync(reminderId), Times.Once);
    }

    [Fact]
    public async Task DeleteReminder_ShouldReturnNotFound_WhenReminderDoesNotExist()
    {
        var reminderId = Guid.NewGuid();
        _reminderServiceMock.Setup(service => service.DeleteReminderAsync(reminderId))
            .ThrowsAsync(new NotFoundException("Reminder not found"));

        var actionResult = await _controller.DeleteReminder(reminderId);

        actionResult.Should().BeOfType<NotFoundObjectResult>();
        _reminderServiceMock.Verify(service => service.DeleteReminderAsync(reminderId), Times.Once);
    }

    [Fact]
    public async Task DeleteReminder_ShouldReturnContextError_WhenInvalid()
    {
        var reminderId = Guid.NewGuid();
        _reminderServiceMock.Setup(service => service.DeleteReminderAsync(reminderId))
            .ThrowsAsync(new ContextException("Context error"));

        var actionResult = await _controller.DeleteReminder(reminderId);

        actionResult.Should().BeOfType<UnprocessableEntityObjectResult>();
        _reminderServiceMock.Verify(service => service.DeleteReminderAsync(reminderId), Times.Once);
    }
}
