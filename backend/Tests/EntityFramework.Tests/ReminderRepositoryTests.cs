using EntityFramework.Context;
using EntityFramework.Models;
using EntityFramework.Repositories.Implementations;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Shared.Models.Output;

namespace Tests.Hestia.Repository;

public class ReminderRepositoryTests
{
    private readonly DbContextOptions<HestiaContext> _dbContextOptions;

    public ReminderRepositoryTests()
    {
        _dbContextOptions = new DbContextOptionsBuilder<HestiaContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Base unique par test
            .Options;
    }

    [Fact]
    public async Task GetAllReminderOutputsAsync_ShouldReturnAllRemindersForColocation()
    {
        // Arrange
        var colocationId = Guid.NewGuid();
        var reminders = new List<Reminder>
                {
                    new Reminder { Id = Guid.NewGuid(), Content = "Reminder 1", Color = "Red", CoordX = 1, CoordY = 2, CoordZ = 3, CreatedBy = "User1", CreatedAt = DateTime.UtcNow, ColocationId = colocationId },
                    new Reminder { Id = Guid.NewGuid(), Content = "Reminder 2", Color = "Blue", CoordX = 4, CoordY = 5, CoordZ = 6, CreatedBy = "User2", CreatedAt = DateTime.UtcNow, ColocationId = colocationId }
                };

        using (var context = new HestiaContext(_dbContextOptions))
        {
            context.Reminder.AddRange(reminders);
            await context.SaveChangesAsync();
        }

        // Act
        List<ReminderOutput>? result;
        using (var context = new HestiaContext(_dbContextOptions))
        {
            var repository = new ReminderRepository(context);
            result = await repository.GetAllReminderOutputsAsync(colocationId);
        }

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result!.Should().BeEquivalentTo(reminders.Select(r => new ReminderOutput
        {
            Id = r.Id,
            Content = r.Content,
            Color = r.Color,
            CoordX = r.CoordX,
            CoordY = r.CoordY,
            CoordZ = r.CoordZ
        }));
    }

    [Fact]
    public async Task GetReminderOutputAsync_ShouldReturnCorrectReminder()
    {
        // Arrange
        var reminder = new Reminder { Id = Guid.NewGuid(), Content = "Important Task", Color = "Green", CoordX = 10, CoordY = 20, CoordZ = 30, CreatedBy = "User3", CreatedAt = DateTime.UtcNow, ColocationId = Guid.NewGuid() };

        using (var context = new HestiaContext(_dbContextOptions))
        {
            context.Reminder.Add(reminder);
            await context.SaveChangesAsync();
        }

        // Act
        ReminderOutput? result;
        using (var context = new HestiaContext(_dbContextOptions))
        {
            var repository = new ReminderRepository(context);
            result = await repository.GetReminderOutputAsync(reminder.Id);
        }

        // Assert
        result.Should().NotBeNull();
        result!.Content.Should().Be(reminder.Content);
    }

    [Fact]
    public async Task AddReminderAsync_ShouldAddReminder()
    {
        // Arrange
        var reminder = new Reminder { Id = Guid.NewGuid(), Content = "Meeting Reminder", Color = "Yellow", CoordX = 5, CoordY = 6, CoordZ = 7, CreatedBy = "User4", CreatedAt = DateTime.UtcNow, ColocationId = Guid.NewGuid() };

        using (var context = new HestiaContext(_dbContextOptions))
        {
            var repository = new ReminderRepository(context);
            await repository.AddReminderAsync(reminder);
            await repository.SaveChangesAsync();
        }

        // Act
        Reminder? result;
        using (var context = new HestiaContext(_dbContextOptions))
        {
            result = await context.Reminder.FindAsync(reminder.Id);
        }

        // Assert
        result.Should().NotBeNull();
        result!.Content.Should().Be("Meeting Reminder");
    }

    [Fact]
    public async Task UpdateReminderAsync_ShouldModifyReminder()
    {
        // Arrange
        var reminder = new Reminder { Id = Guid.NewGuid(), Content = "Initial Content", Color = "Orange", CoordX = 8, CoordY = 9, CoordZ = 10, CreatedBy = "User5", CreatedAt = DateTime.UtcNow, ColocationId = Guid.NewGuid() };

        using (var context = new HestiaContext(_dbContextOptions))
        {
            context.Reminder.Add(reminder);
            await context.SaveChangesAsync();
        }

        using (var context = new HestiaContext(_dbContextOptions))
        {
            var repository = new ReminderRepository(context);
            reminder.Content = "Updated Content";
            await repository.UpdateReminderAsync(reminder);
            await repository.SaveChangesAsync();
        }

        // Act
        Reminder? result;
        using (var context = new HestiaContext(_dbContextOptions))
        {
            result = await context.Reminder.FindAsync(reminder.Id);
        }

        // Assert
        result.Should().NotBeNull();
        result!.Content.Should().Be("Updated Content");
    }

    [Fact]
    public async Task DeleteReminderAsync_ShouldRemoveReminder()
    {
        // Arrange
        var reminder = new Reminder { Id = Guid.NewGuid(), Content = "To be deleted", Color = "Pink", CoordX = 12, CoordY = 15, CoordZ = 20, CreatedBy = "User6", CreatedAt = DateTime.UtcNow, ColocationId = Guid.NewGuid() };

        using (var context = new HestiaContext(_dbContextOptions))
        {
            context.Reminder.Add(reminder);
            await context.SaveChangesAsync();
        }

        using (var context = new HestiaContext(_dbContextOptions))
        {
            var repository = new ReminderRepository(context);
            await repository.DeleteReminderAsync(reminder);
            await repository.SaveChangesAsync();
        }

        // Act
        Reminder? result;
        using (var context = new HestiaContext(_dbContextOptions))
        {
            result = await context.Reminder.FindAsync(reminder.Id);
        }

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetReminderFromListOfId_ShouldReturnMatchingReminders()
    {
        // Arrange
        var reminders = new List<Reminder>
                {
                    new Reminder { Id = Guid.NewGuid(), Content = "Reminder A", Color = "Red", CoordX = 1, CoordY = 2, CoordZ = 3, CreatedBy = "User1", CreatedAt = DateTime.UtcNow, ColocationId = Guid.NewGuid() },
                    new Reminder { Id = Guid.NewGuid(), Content = "Reminder B", Color = "Blue", CoordX = 4, CoordY = 5, CoordZ = 6, CreatedBy = "User2", CreatedAt = DateTime.UtcNow, ColocationId = Guid.NewGuid() }
                };

        using (var context = new HestiaContext(_dbContextOptions))
        {
            context.Reminder.AddRange(reminders);
            await context.SaveChangesAsync();
        }

        var reminderIds = reminders.Select(r => r.Id).ToList();

        // Act
        List<Reminder>? result;
        using (var context = new HestiaContext(_dbContextOptions))
        {
            var repository = new ReminderRepository(context);
            result = await repository.GetReminderFromListOfId(reminderIds);
        }

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result!.Select(r => r.Id).Should().BeEquivalentTo(reminderIds);
    }

    [Fact]
    public async Task GetReminderAsync_ShouldReturnCorrectReminder()
    {
        // Arrange
        var reminder = new Reminder
        {
            Id = Guid.NewGuid(),
            Content = "Fetch this reminder",
            Color = "Purple",
            CoordX = 1,
            CoordY = 2,
            CoordZ = 3,
            CreatedBy = "UserTest",
            CreatedAt = DateTime.UtcNow,
            ColocationId = Guid.NewGuid()
        };

        using (var context = new HestiaContext(_dbContextOptions))
        {
            context.Reminder.Add(reminder);
            await context.SaveChangesAsync();
        }

        // Act
        Reminder? result;
        using (var context = new HestiaContext(_dbContextOptions))
        {
            var repository = new ReminderRepository(context);
            result = await repository.GetReminderAsync(reminder.Id);
        }

        // Assert
        result.Should().NotBeNull();
        result!.Content.Should().Be(reminder.Content);
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldPersistData()
    {
        // Arrange
        var reminder = new Reminder
        {
            Id = Guid.NewGuid(),
            Content = "Save this",
            Color = "Brown",
            CoordX = 10,
            CoordY = 20,
            CoordZ = 30,
            CreatedBy = "UserSave",
            CreatedAt = DateTime.UtcNow,
            ColocationId = Guid.NewGuid()
        };

        using (var context = new HestiaContext(_dbContextOptions))
        {
            var repository = new ReminderRepository(context);
            await repository.AddReminderAsync(reminder);
            await repository.SaveChangesAsync();
        }

        // Act
        Reminder? result;
        using (var context = new HestiaContext(_dbContextOptions))
        {
            result = await context.Reminder.FindAsync(reminder.Id);
        }

        // Assert
        result.Should().NotBeNull();
        result!.Content.Should().Be(reminder.Content);
    }
}