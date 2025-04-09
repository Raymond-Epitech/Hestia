using Business.Services;
using EntityFramework.Models;
using EntityFramework.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Shared.Exceptions;
using Shared.Models.Output;

namespace Tests.Hestia.Service;

public class ChoreServiceTests
{
    private readonly ChoreService _choreService;
    private readonly Mock<ILogger<ChoreService>> _loggerMock;
    private readonly Mock<IRepository<Chore>> _choreRepositoryMock;
    private readonly Mock<IRepository<ChoreMessage>> _choreMessageRepositoryMock;
    private readonly Mock<IRepository<User>> _userRepositoryMock;
    private readonly Mock<IRepository<ChoreEnrollment>> _choreEnrollmentRepositoryMock;

    public ChoreServiceTests()
    {
        _loggerMock = new Mock<ILogger<ChoreService>>();
        _choreRepositoryMock = new Mock<IRepository<Chore>>();
        _choreMessageRepositoryMock = new Mock<IRepository<ChoreMessage>>();
        _userRepositoryMock = new Mock<IRepository<User>>();
        _choreEnrollmentRepositoryMock = new Mock<IRepository<ChoreEnrollment>>();

        _choreService = new ChoreService(
            _loggerMock.Object,
            _choreRepositoryMock.Object,
            _choreMessageRepositoryMock.Object,
            _userRepositoryMock.Object,
            _choreEnrollmentRepositoryMock.Object
        );
    }

    // GET ALL CHORE

    [Fact]
    public async Task GetAllChoresAsync_ShouldReturnList_WhenChoresExist()
    {
        var colocationId = Guid.NewGuid();
        var expectedChores = new List<ChoreOutput>
        {
            new ChoreOutput {
                Id = Guid.NewGuid(),
                Title = "Test Chore",
                Description = "Test Description",
                CreatedBy = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(2),
                IsDone = false
            }
        };

        var mockData = new List<Chore>
        {
            new Chore
            {
                Id = expectedChores.First().Id,
                CreatedBy = expectedChores.First().CreatedBy,
                CreatedAt = expectedChores.First().CreatedAt,
                Title = expectedChores.First().Title,
                Description = expectedChores.First().Description,
                DueDate = expectedChores.First().DueDate,
                IsDone = expectedChores.First().IsDone,
                ColocationId = colocationId
            }}
        .AsQueryable();

        //_choreRepositoryMock.SetupGet(repo => repo.Query).Returns(mockData);

        var result = await _choreService.GetAllChoresAsync(colocationId);

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedChores);
    }

    // GET CHORE

    [Fact]
    public async Task GetChoreAsync_ShouldReturnChore_WhenChoreExists()
    {
        var choreId = Guid.NewGuid();
        var expectedChore = new ChoreOutput
        {
            Id = choreId,
            Title = "Test Chore",
            CreatedBy = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(2),
            IsDone = false
        };

        /*_repositoryMock.Setup(repo => repo.GetByIdAsTypeAsync(choreId, user => new ChoreOutput
        {
            Id = user.Id,
            CreatedBy = user.CreatedBy,
            CreatedAt = user.CreatedAt,
            DueDate = user.DueDate,
            Title = user.Title,
            Description = user.Description,
            IsDone = user.IsDone
        })).ReturnsAsync(expectedChore);*/

        var result = await _choreService.GetChoreAsync(choreId);

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedChore);
    }

    [Fact]
    public async Task GetChoreAsync_ShouldThrowNotFound_WhenChoreDoesNotExist()
    {
        var choreId = Guid.NewGuid();
        //_repositoryMock.Setup(repo => repo.GetByIdAsTypeAsync(choreId, u => null as ChoreOutput)).ReturnsAsync(null as ChoreOutput);

        await Assert.ThrowsAsync<NotFoundException>(() => _choreService.GetChoreAsync(choreId));
    }

    // ADD CHORE

    /*
    [Fact]
    public async Task AddChoreAsync_ShouldReturnId_WhenChoreIsAdded()
    {
        var choreInput = new ChoreInput { ColocationId = Guid.NewGuid(), CreatedBy = Guid.NewGuid(), DueDate = DateTime.UtcNow.AddDays(3), Title = "New Chore", IsDone = false };
        var choreId = Guid.NewGuid();

        _choreRepoMock.Setup(repo => repo.AddChoreAsync(It.IsAny<EntityFramework.Models.Chore>()))
            .Callback<EntityFramework.Models.Chore>(c => c.Id = choreId)
            .Returns(Task.CompletedTask);

        var result = await _choreService.AddChoreAsync(choreInput);

        result.Should().Be(choreId);
        await Task.Delay(1);
    }

    // UPDATE CHORE

    [Fact]
    public async Task UpdateChoreAsync_ShouldUpdate_WhenChoreExists()
    {
        var choreUpdate = new ChoreUpdate { Id = Guid.NewGuid(), DueDate = DateTime.UtcNow.AddDays(3), Title = "Updated Chore", IsDone = true };
        var existingChore = new EntityFramework.Models.Chore { Id = choreUpdate.Id, Title = "Old Chore", IsDone = false };

        _choreRepoMock.Setup(repo => repo.GetChoreByIdAsync(choreUpdate.Id)).ReturnsAsync(existingChore);
        _choreRepoMock.Setup(repo => repo.SaveChangesAsync());

        await _choreService.UpdateChoreAsync(choreUpdate);

        existingChore.Title.Should().Be(choreUpdate.Title);
        existingChore.IsDone.Should().Be(choreUpdate.IsDone);
        await Task.Delay(1);
    }

    [Fact]
    public async Task UpdateChoreAsync_ShouldThrowNotFound_WhenChoreDoesNotExist()
    {
        var choreUpdate = new ChoreUpdate { Id = Guid.NewGuid(), Title = "Updated Title" };

        _choreRepoMock.Setup(repo => repo.GetChoreByIdAsync(choreUpdate.Id)).ReturnsAsync((EntityFramework.Models.Chore?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _choreService.UpdateChoreAsync(choreUpdate));
    }

    // DELETE CHORE

    [Fact]
    public async Task DeleteChoreAsync_ShouldDelete_WhenChoreExists()
    {
        var choreId = Guid.NewGuid();
        var existingChore = new EntityFramework.Models.Chore { Id = choreId };

        _choreRepoMock.Setup(repo => repo.GetChoreByIdAsync(choreId)).ReturnsAsync(existingChore);
        _choreRepoMock.Setup(repo => repo.DeleteChoreAsync(existingChore)).Returns(Task.CompletedTask);

        await _choreService.DeleteChoreAsync(choreId);
    }

    [Fact]
    public async Task DeleteChoreAsync_ShouldThrowNotFound_WhenChoreDoesNotExist()
    {
        var choreId = Guid.NewGuid();
        _choreRepoMock.Setup(repo => repo.GetChoreByIdAsync(choreId)).ReturnsAsync((EntityFramework.Models.Chore?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _choreService.DeleteChoreAsync(choreId));
    }

    // GET ALL CHORE MESSAGE

    [Fact]
    public async Task GetChoreMessagesAsync_ShouldReturnList_WhenMessagesExist()
    {
        var choreId = Guid.NewGuid();
        var expectedMessages = new List<ChoreMessageOutput>
        {
            new ChoreMessageOutput { Id = Guid.NewGuid(), CreatedBy = "User", CreatedAt = DateTime.UtcNow, Content = "Message 1" }
        };

        _choreRepoMock.Setup(repo => repo.GetAllChoreMessageOutputByChoreIdAsync(choreId)).ReturnsAsync(expectedMessages);

        var result = await _choreService.GetChoreMessageFromChoreAsync(choreId);

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedMessages);
    }

    [Fact]
    public async Task GetChoreMessagesAsync_ShouldThrowNotFound_WhenNoMessagesExist()
    {
        var choreId = Guid.NewGuid();
        _choreRepoMock
            .Setup(repo => repo.GetAllChoreMessageOutputByChoreIdAsync(choreId))
            .ReturnsAsync((List<ChoreMessageOutput>?)null!);


        await Assert.ThrowsAsync<NotFoundException>(() => _choreService.GetChoreMessageFromChoreAsync(choreId));
    }

    // ADD CHORE MESSAGE

    [Fact]
    public async Task AddChoreMessageAsync_ShouldReturnId_WhenMessageIsAdded()
    {
        var messageInput = new ChoreMessageInput { ChoreId = Guid.NewGuid(), CreatedBy = "User", Content = "New Message" };
        var messageId = Guid.NewGuid();

        _choreRepoMock.Setup(repo => repo.AddChoreMessageAsync(It.IsAny<EntityFramework.Models.ChoreMessage>()))
            .Callback<EntityFramework.Models.ChoreMessage>(m => m.Id = messageId)
            .Returns(Task.CompletedTask);

        var result = await _choreService.AddChoreMessageAsync(messageInput);

        result.Should().Be(messageId);
    }

    // DELETE CHORE MESSAGE

    [Fact]
    public async Task DeleteChoreMessagesByChoreIdAsync_ShouldDelete_WhenMessagesExist()
    {
        var choreId = Guid.NewGuid();
        var messages = new List<EntityFramework.Models.ChoreMessage>
        {
            new EntityFramework.Models.ChoreMessage { Id = Guid.NewGuid(), ChoreId = choreId }
        };

        _choreRepoMock.Setup(repo => repo.GetChoreMessageFromChoreId(choreId)).ReturnsAsync(messages);
        _choreRepoMock.Setup(repo => repo.DeleteRangeChoreMessageFromChoreId(messages)).Returns(Task.CompletedTask);

        await _choreService.DeleteChoreMessageByChoreIdAsync(choreId);
    }

    [Fact]
    public async Task DeleteChoreMessagesByChoreIdAsync_ShouldThrowNotFound_WhenMessagesDoNotExist()
    {
        var choreId = Guid.NewGuid();
        _choreRepoMock.Setup(repo => repo.GetChoreMessageFromChoreId(choreId)).ReturnsAsync((List<EntityFramework.Models.ChoreMessage>?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _choreService.DeleteChoreMessageByChoreIdAsync(choreId));
    }

    // GET CHORE MESSAGE BY

    [Fact]
    public async Task GetChoreMessageFromChoreAsync_ShouldReturnList_WhenMessagesExist()
    {
        var choreId = Guid.NewGuid();
        var expectedMessages = new List<ChoreMessageOutput>
        {
            new ChoreMessageOutput { Id = Guid.NewGuid(), CreatedBy = "User", CreatedAt = DateTime.UtcNow, Content = "Message 1" }
        };

        _choreRepoMock.Setup(repo => repo.GetAllChoreMessageOutputByChoreIdAsync(choreId)).ReturnsAsync(expectedMessages);

        var result = await _choreService.GetChoreMessageFromChoreAsync(choreId);

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedMessages);
    }

    [Fact]
    public async Task GetChoreMessageFromChoreAsync_ShouldThrowNotFound_WhenNoMessagesExist()
    {
        var choreId = Guid.NewGuid();
        _choreRepoMock.Setup(repo => repo.GetAllChoreMessageOutputByChoreIdAsync(choreId)).ReturnsAsync((List<ChoreMessageOutput>?)null!);

        await Assert.ThrowsAsync<NotFoundException>(() => _choreService.GetChoreMessageFromChoreAsync(choreId));
    }

    // GET USER BY

    [Fact]
    public async Task GetUserFromChore_ShouldReturnList_WhenUsersExist()
    {
        var choreId = Guid.NewGuid();
        var expectedUsers = new List<UserOutput>
    {
        new UserOutput { Id = Guid.NewGuid(), Username = "User1", Email = "user1@example.com" }
    };

        _choreRepoMock.Setup(repo => repo.GetEnrolledUserOutputFromChoreIdAsync(choreId)).ReturnsAsync(expectedUsers);

        var result = await _choreService.GetUserFromChore(choreId);

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedUsers);
    }

    // GET CHORE BY

    [Fact]
    public async Task GetChoreFromUser_ShouldReturnList_WhenChoresExist()
    {
        var userId = Guid.NewGuid();
        var expectedChores = new List<ChoreOutput>
    {
        new ChoreOutput { Id = Guid.NewGuid(), Title = "Test Chore", CreatedBy = "User", CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(2), IsDone = false }
    };

        _choreRepoMock.Setup(repo => repo.GetEnrolledChoreOutputFromUserIdAsync(userId)).ReturnsAsync(expectedChores);

        var result = await _choreService.GetChoreFromUser(userId);

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedChores);
    }

    // ENROLL

    [Fact]
    public async Task EnrollToChore_ShouldSucceed_WhenValid()
    {
        var userId = Guid.NewGuid();
        var choreId = Guid.NewGuid();

        _choreRepoMock.Setup(repo => repo.AddChoreEnrollmentAsync(It.IsAny<EntityFramework.Models.ChoreEnrollment>()))
            .Returns(Task.CompletedTask);
        _choreRepoMock.Setup(repo => repo.SaveChangesAsync()).Returns(Task.CompletedTask);

        await _choreService.EnrollToChore(userId, choreId);

        _choreRepoMock.Verify(repo => repo.AddChoreEnrollmentAsync(It.IsAny<EntityFramework.Models.ChoreEnrollment>()), Times.Once);
        _choreRepoMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
    }

    // UNENROLL

    [Fact]
    public async Task UnenrollToChore_ShouldRemoveEnrollment_WhenEnrollmentExists()
    {
        var userId = Guid.NewGuid();
        var choreId = Guid.NewGuid();
        var enrollment = new EntityFramework.Models.ChoreEnrollment { UserId = userId, ChoreId = choreId };

        _choreRepoMock.Setup(repo => repo.GetChoreEnrollmentByUserIdAndChoreIdAsync(userId, choreId)).ReturnsAsync(enrollment);
        _choreRepoMock.Setup(repo => repo.RemoveChoreEnrollmentAsync(enrollment)).Returns(Task.CompletedTask);
        _choreRepoMock.Setup(repo => repo.SaveChangesAsync()).Returns(Task.CompletedTask);

        await _choreService.UnenrollToChore(userId, choreId);

        _choreRepoMock.Verify(repo => repo.RemoveChoreEnrollmentAsync(enrollment), Times.Once);
        _choreRepoMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UnenrollToChore_ShouldThrowNotFound_WhenEnrollmentDoesNotExist()
    {
        var userId = Guid.NewGuid();
        var choreId = Guid.NewGuid();

        _choreRepoMock.Setup(repo => repo.GetChoreEnrollmentByUserIdAndChoreIdAsync(userId, choreId)).ReturnsAsync((EntityFramework.Models.ChoreEnrollment?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _choreService.UnenrollToChore(userId, choreId));
    }
    */
}