using EntityFramework.Context;
using EntityFramework.Models;
using EntityFramework.Repositories.Implementations;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Shared.Models.Output;

namespace Tests.Hestia.Repository;

public class ChoreRepositoryTests
{
    private readonly DbContextOptions<HestiaContext> _dbContextOptions;

    public ChoreRepositoryTests()
    {
        _dbContextOptions = new DbContextOptionsBuilder<HestiaContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Base unique par test
            .Options;
    }

    [Fact]
    public async Task GetAllChoreOutputsAsync_ShouldReturnAllChoresForColocation()
    {
        // Arrange
        var colocationId = Guid.NewGuid();
        var chores = new List<Chore>
        {
            new Chore { Id = Guid.NewGuid(), Title = "Task 1", CreatedBy = "User1", CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(3), IsDone = false, ColocationId = colocationId },
            new Chore { Id = Guid.NewGuid(), Title = "Task 2", CreatedBy = "User2", CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(5), IsDone = true, ColocationId = colocationId }
        };

        using (var context = new HestiaContext(_dbContextOptions))
        {
            context.Chores.AddRange(chores);
            await context.SaveChangesAsync();
        }

        // Act
        List<ChoreOutput>? result;
        using (var context = new HestiaContext(_dbContextOptions))
        {
            var repository = new ChoreRepository(context);
            result = await repository.GetAllChoreOutputsAsync(colocationId);
        }

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result!.Should().BeEquivalentTo(chores.Select(c => new ChoreOutput
        {
            Id = c.Id,
            CreatedBy = c.CreatedBy,
            CreatedAt = c.CreatedAt,
            DueDate = c.DueDate,
            Title = c.Title,
            Description = c.Description,
            IsDone = c.IsDone
        }), options => options.Excluding(x => x.CreatedAt));
    }

    [Fact]
    public async Task GetChoreOutputByIdAsync_ShouldReturnCorrectChore()
    {
        // Arrange
        var chore = new Chore { Id = Guid.NewGuid(), Title = "Clean Kitchen", CreatedBy = "User1", CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(2), IsDone = false };

        using (var context = new HestiaContext(_dbContextOptions))
        {
            context.Chores.Add(chore);
            await context.SaveChangesAsync();
        }

        // Act
        ChoreOutput? result;
        using (var context = new HestiaContext(_dbContextOptions))
        {
            var repository = new ChoreRepository(context);
            result = await repository.GetChoreOutputByIdAsync(chore.Id);
        }

        // Assert
        result.Should().NotBeNull();
        result!.Title.Should().Be(chore.Title);
    }

    [Fact]
    public async Task AddChoreAsync_ShouldAddNewChore()
    {
        // Arrange
        var chore = new Chore { Id = Guid.NewGuid(), Title = "Vacuuming", CreatedBy = "User2", CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(1), IsDone = false };

        using (var context = new HestiaContext(_dbContextOptions))
        {
            var repository = new ChoreRepository(context);
            await repository.AddChoreAsync(chore);
            await repository.SaveChangesAsync();
        }

        // Act
        Chore? result;
        using (var context = new HestiaContext(_dbContextOptions))
        {
            result = await context.Chores.FindAsync(chore.Id);
        }

        // Assert
        result.Should().NotBeNull();
        result!.Title.Should().Be(chore.Title);
    }

    [Fact]
    public async Task UpdateChoreAsync_ShouldModifyChore()
    {
        // Arrange
        var chore = new Chore { Id = Guid.NewGuid(), Title = "Dishes", CreatedBy = "User1", CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(1), IsDone = false };

        using (var context = new HestiaContext(_dbContextOptions))
        {
            context.Chores.Add(chore);
            await context.SaveChangesAsync();
        }

        using (var context = new HestiaContext(_dbContextOptions))
        {
            var repository = new ChoreRepository(context);
            chore.Title = "Dishes & Clean Table";
            await repository.UpdateChoreAsync(chore);
            await repository.SaveChangesAsync();
        }

        // Act
        Chore? result;
        using (var context = new HestiaContext(_dbContextOptions))
        {
            result = await context.Chores.FindAsync(chore.Id);
        }

        // Assert
        result.Should().NotBeNull();
        result!.Title.Should().Be("Dishes & Clean Table");
    }

    [Fact]
    public async Task DeleteChoreAsync_ShouldRemoveChore()
    {
        // Arrange
        var chore = new Chore { Id = Guid.NewGuid(), Title = "Trash", CreatedBy = "User3", CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(2), IsDone = false };

        using (var context = new HestiaContext(_dbContextOptions))
        {
            context.Chores.Add(chore);
            await context.SaveChangesAsync();
        }

        using (var context = new HestiaContext(_dbContextOptions))
        {
            var repository = new ChoreRepository(context);
            await repository.DeleteChoreAsync(chore);
            await repository.SaveChangesAsync();
        }

        // Act
        Chore? result;
        using (var context = new HestiaContext(_dbContextOptions))
        {
            result = await context.Chores.FindAsync(chore.Id);
        }

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllChoreMessageOutputByChoreIdAsync_ShouldReturnMessages()
    {
        // Arrange
        var choreId = Guid.NewGuid();
        var messages = new List<ChoreMessage>
            {
                new ChoreMessage { Id = Guid.NewGuid(), ChoreId = choreId, CreatedBy = "User1", CreatedAt = DateTime.UtcNow, Content = "Message 1" },
                new ChoreMessage { Id = Guid.NewGuid(), ChoreId = choreId, CreatedBy = "User2", CreatedAt = DateTime.UtcNow, Content = "Message 2" }
            };

        using (var context = new HestiaContext(_dbContextOptions))
        {
            context.ChoreMessages.AddRange(messages);
            await context.SaveChangesAsync();
        }

        // Act
        List<ChoreMessageOutput>? result;
        using (var context = new HestiaContext(_dbContextOptions))
        {
            var repository = new ChoreRepository(context);
            result = await repository.GetAllChoreMessageOutputByChoreIdAsync(choreId);
        }

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result!.Should().BeEquivalentTo(messages.Select(m => new ChoreMessageOutput
        {
            Id = m.Id,
            CreatedBy = m.CreatedBy,
            CreatedAt = m.CreatedAt,
            Content = m.Content
        }), options => options.Excluding(x => x.CreatedAt));
    }

    [Fact]
    public async Task GetEnrolledUserOutputFromChoreIdAsync_ShouldReturnUsers()
    {
        // Arrange
        var choreId = Guid.NewGuid();
        var user = new User { Id = Guid.NewGuid(), Username = "User1", Email = "user1@example.com", CreatedAt = DateTime.UtcNow, LastConnection = DateTime.UtcNow, PathToProfilePicture = "default.jpg" , ColocationId = Guid.NewGuid()};
        var enrollment = new ChoreEnrollment { ChoreId = choreId, UserId = user.Id, CreatedAt = DateTime.UtcNow };

        using (var context = new HestiaContext(_dbContextOptions))
        {
            context.Users.Add(user);
            context.ChoreEnrollments.Add(enrollment);
            await context.SaveChangesAsync();
        }

        // Act
        List<UserOutput>? result;
        using (var context = new HestiaContext(_dbContextOptions))
        {
            var repository = new ChoreRepository(context);
            result = await repository.GetEnrolledUserOutputFromChoreIdAsync(choreId);
        }

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result!.First().Username.Should().Be("User1");
    }

    [Fact]
    public async Task AddChoreEnrollmentAsync_ShouldAddEnrollment()
    {
        // Arrange
        var chore = new Chore { Id = Guid.NewGuid(), Title = "Test Chore", CreatedBy = "User1", CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(1), IsDone = false };
        var user = new User { Id = Guid.NewGuid(), Username = "Test User", Email = "test@example.com", CreatedAt = DateTime.UtcNow, LastConnection = DateTime.UtcNow, PathToProfilePicture = "default.jpg", ColocationId = Guid.NewGuid() };

        var enrollment = new ChoreEnrollment { ChoreId = chore.Id, UserId = user.Id, CreatedAt = DateTime.UtcNow };

        using (var context = new HestiaContext(_dbContextOptions))
        {
            context.Chores.Add(chore);
            context.Users.Add(user);
            await context.SaveChangesAsync();
        }

        using (var context = new HestiaContext(_dbContextOptions))
        {
            var repository = new ChoreRepository(context);
            await repository.AddChoreEnrollmentAsync(enrollment);
            await repository.SaveChangesAsync();
        }

        // Act
        ChoreEnrollment? result;
        using (var context = new HestiaContext(_dbContextOptions))
        {
            result = await context.ChoreEnrollments.FirstOrDefaultAsync(x => x.ChoreId == enrollment.ChoreId && x.UserId == enrollment.UserId);
        }

        // Assert
        result.Should().NotBeNull();
        result!.ChoreId.Should().Be(enrollment.ChoreId);
        result!.UserId.Should().Be(enrollment.UserId);
    }

    [Fact]
    public async Task RemoveChoreEnrollmentAsync_ShouldDeleteEnrollment()
    {
        // Arrange
        var choreId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var enrollment = new ChoreEnrollment { ChoreId = choreId, UserId = userId, CreatedAt = DateTime.UtcNow };

        using (var context = new HestiaContext(_dbContextOptions))
        {
            context.ChoreEnrollments.Add(enrollment);
            await context.SaveChangesAsync();
        }

        using (var context = new HestiaContext(_dbContextOptions))
        {
            var repository = new ChoreRepository(context);
            await repository.RemoveChoreEnrollmentAsync(enrollment);
            await repository.SaveChangesAsync();
        }

        // Act
        ChoreEnrollment? result;
        using (var context = new HestiaContext(_dbContextOptions))
        {
            result = await context.ChoreEnrollments.FindAsync(choreId, userId);
        }

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetChoreEnrollmentByUserIdAndChoreIdAsync_ShouldReturnEnrollment()
    {
        // Arrange
        var choreId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var enrollment = new ChoreEnrollment { ChoreId = choreId, UserId = userId, CreatedAt = DateTime.UtcNow };

        using (var context = new HestiaContext(_dbContextOptions))
        {
            context.ChoreEnrollments.Add(enrollment);
            await context.SaveChangesAsync();
        }

        // Act
        ChoreEnrollment? result;
        using (var context = new HestiaContext(_dbContextOptions))
        {
            var repository = new ChoreRepository(context);
            result = await repository.GetChoreEnrollmentByUserIdAndChoreIdAsync(userId, choreId);
        }

        // Assert
        result.Should().NotBeNull();
        result!.ChoreId.Should().Be(choreId);
        result!.UserId.Should().Be(userId);
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldPersistData()
    {
        // Arrange
        var chore = new Chore { Id = Guid.NewGuid(), Title = "Test Save", CreatedBy = "UserTest", CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(1), IsDone = false };

        using (var context = new HestiaContext(_dbContextOptions))
        {
            var repository = new ChoreRepository(context);
            await repository.AddChoreAsync(chore);
            await repository.SaveChangesAsync();
        }

        // Act
        Chore? result;
        using (var context = new HestiaContext(_dbContextOptions))
        {
            result = await context.Chores.FindAsync(chore.Id);
        }

        // Assert
        result.Should().NotBeNull();
    }
}
