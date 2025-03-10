using Business.Services;
using EntityFramework.Repositories.Interfaces;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Moq;
using Shared.Exceptions;
using Shared.Models.Input;
using Shared.Models.Output;
using Shared.Models.Update;

namespace Tests.Hestia.Service;

public class ReminderServiceTests
{
    private readonly ReminderService _reminderService;
    private readonly Mock<ILogger<ReminderService>> _loggerMock;
    private readonly Mock<IReminderRepository> _reminderRepoMock;

    public ReminderServiceTests()
    {
        _reminderRepoMock = new Mock<IReminderRepository>();
        _loggerMock = new Mock<ILogger<ReminderService>>();
        _reminderService = new ReminderService(_loggerMock.Object, _reminderRepoMock.Object);
    }

    // GET ALL REMINDER

    [Fact]
    public async Task GetAllRemindersAsync_ShouldReturnList_WhenRemindersExist()
    {
        var colocationId = Guid.NewGuid();
        var expectedReminders = new List<ReminderOutput>
        {
            new ReminderOutput
            {
                Id = Guid.NewGuid(),
                Content = "Reminder 1",
                Color = "Red",
                CreatedBy = "User",
                CreatedAt = DateTime.UtcNow
            }
        };

        _reminderRepoMock.Setup(repo => repo.GetAllReminderOutputsAsync(colocationId)).ReturnsAsync(expectedReminders);

        var result = await _reminderService.GetAllRemindersAsync(colocationId);

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedReminders);
    }

    // GET REMINDER

    [Fact]
    public async Task GetReminderAsync_ShouldReturnReminder_WhenReminderExists()
    {
        var reminderId = Guid.NewGuid();
        var expectedReminder = new ReminderOutput
        {
            Id = reminderId,
            Content = "Reminder 1",
            Color = "Blue",
            CreatedBy = "User",
            CreatedAt = DateTime.UtcNow
        };

        _reminderRepoMock.Setup(repo => repo.GetReminderOutputAsync(reminderId)).ReturnsAsync(expectedReminder);

        var result = await _reminderService.GetReminderAsync(reminderId);

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedReminder);
    }

    [Fact]
    public async Task GetReminderAsync_ShouldThrowNotFound_WhenReminderDoesNotExist()
    {
        var reminderId = Guid.NewGuid();
        _reminderRepoMock.Setup(repo => repo.GetReminderOutputAsync(reminderId)).ReturnsAsync((ReminderOutput?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _reminderService.GetReminderAsync(reminderId));
    }

    // ADD REMINDER

    [Fact]
    public async Task AddReminderAsync_ShouldReturnId_WhenReminderIsAdded()
    {
        var reminderInput = new ReminderInput
        {
            ColocationId = Guid.NewGuid(),
            CreatedBy = "User",
            Content = "Test Reminder",
            Color = "Green"
        };
        var reminderId = Guid.NewGuid();

        _reminderRepoMock.Setup(repo => repo.AddReminderAsync(It.IsAny<EntityFramework.Models.Reminder>()))
            .Callback<EntityFramework.Models.Reminder>(r => r.Id = reminderId)
            .Returns(Task.CompletedTask);

        var result = await _reminderService.AddReminderAsync(reminderInput);

        result.Should().Be(reminderId);
    }

    // UPDATE REMINDER

    [Fact]
    public async Task UpdateReminderAsync_ShouldUpdate_WhenReminderExists()
    {
        var reminderUpdate = new ReminderUpdate
        {
            Id = Guid.NewGuid(),
            Content = "Updated Reminder",
            Color = "Yellow"
        };
        var existingReminder = new EntityFramework.Models.Reminder { Id = reminderUpdate.Id, Content = "Old Reminder", Color = "Blue" };

        _reminderRepoMock.Setup(repo => repo.GetReminderAsync(reminderUpdate.Id)).ReturnsAsync(existingReminder);
        _reminderRepoMock.Setup(repo => repo.SaveChangesAsync()).Returns(Task.CompletedTask);

        await _reminderService.UpdateReminderAsync(reminderUpdate);

        existingReminder.Content.Should().Be(reminderUpdate.Content);
        existingReminder.Color.Should().Be(reminderUpdate.Color);
    }

    [Fact]
    public async Task UpdateReminderAsync_ShouldThrowNotFound_WhenReminderDoesNotExist()
    {
        var reminderUpdate = new ReminderUpdate
        {
            Id = Guid.NewGuid(),
            Content = "New Content"
        };

        _reminderRepoMock.Setup(repo => repo.GetReminderAsync(reminderUpdate.Id)).ReturnsAsync((EntityFramework.Models.Reminder?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _reminderService.UpdateReminderAsync(reminderUpdate));
    }

    // UPDATE RANGE REMINDER

    [Fact]
    public async Task UpdateRangeReminderAsync_ShouldUpdate_WhenRemindersExist()
    {
        var updates = new List<ReminderUpdate>
        {
            new ReminderUpdate
            {
                Id = Guid.NewGuid(),
                Content = "Updated 1",
                Color = "Red"
            },
            new ReminderUpdate
            {
                Id = Guid.NewGuid(),
                Content = "Updated 2",
                Color = "Blue"
            }
        };

        var reminders = updates.Select(u => new EntityFramework.Models.Reminder
        {
            Id = u.Id,
            Content = "Old",
            Color = "Green"
        }).ToList();

        var mockTransaction = new Mock<IDbContextTransaction>();
        mockTransaction.Setup(t => t.Commit()).Verifiable();
        mockTransaction.Setup(t => t.Rollback()).Verifiable();

        _reminderRepoMock.Setup(repo => repo.GetReminderFromListOfId(It.IsAny<List<Guid>>())).ReturnsAsync(reminders);
        _reminderRepoMock.Setup(repo => repo.BeginTransactionAsync()).ReturnsAsync(mockTransaction.Object);
        _reminderRepoMock.Setup(repo => repo.SaveChangesAsync()).Returns(Task.CompletedTask);

        await _reminderService.UpdateRangeReminderAsync(updates);

        mockTransaction.Verify(t => t.Commit(), Times.Once);
        _reminderRepoMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
    }


    [Fact]
    public async Task UpdateRangeReminderAsync_ShouldThrowNotFound_WhenOneOrMoreRemindersAreMissing()
    {
        var updates = new List<ReminderUpdate>
        {
            new ReminderUpdate
            {
                Id = Guid.NewGuid(),
                Content = "Updated 1",
                Color = "Red"
            },
            new ReminderUpdate
            {
                Id = Guid.NewGuid(),
                Content = "Updated 2",
                Color = "Blue"
            }
        };

        _reminderRepoMock.Setup(repo => repo.GetReminderFromListOfId(It.IsAny<List<Guid>>())).ReturnsAsync(new List<EntityFramework.Models.Reminder>());

        await Assert.ThrowsAsync<NotFoundException>(() => _reminderService.UpdateRangeReminderAsync(updates));
    }

    // DELETE REMINDER

    [Fact]
    public async Task DeleteReminderAsync_ShouldDelete_WhenReminderExists()
    {
        var reminderId = Guid.NewGuid();
        var existingReminder = new EntityFramework.Models.Reminder { Id = reminderId };

        _reminderRepoMock.Setup(repo => repo.GetReminderAsync(reminderId)).ReturnsAsync(existingReminder);
        _reminderRepoMock.Setup(repo => repo.DeleteReminderAsync(existingReminder)).Returns(Task.CompletedTask);

        await _reminderService.DeleteReminderAsync(reminderId);
    }

    [Fact]
    public async Task DeleteReminderAsync_ShouldThrowNotFound_WhenReminderDoesNotExist()
    {
        var reminderId = Guid.NewGuid();
        _reminderRepoMock.Setup(repo => repo.GetReminderAsync(reminderId)).ReturnsAsync((EntityFramework.Models.Reminder?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _reminderService.DeleteReminderAsync(reminderId));
    }
}
