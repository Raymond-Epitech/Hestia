using Business.Services;
using EntityFramework.Models;
using EntityFramework.Repositories;
using FluentAssertions;
using LazyCache;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Shared.Models.Input;
using Shared.Models.Output;

namespace Tests.Hestia.Business;

public class ChoreServiceTests
{
    private readonly Mock<ILogger<ChoreService>> _loggerMock = new();
    private readonly Mock<IRepository<Chore>> _choreRepositoryMock = new();
    private readonly Mock<IRepository<ChoreMessage>> _choreMessageRepositoryMock = new();
    private readonly Mock<IRepository<User>> _userRepositoryMock = new();
    private readonly Mock<IRepository<ChoreEnrollment>> _choreEnrollmentRepositoryMock = new();
    private readonly Mock<IAppCache> _cacheMock = new();

    private readonly ChoreService _service;

    public ChoreServiceTests()
    {
        _service = new ChoreService(
            _loggerMock.Object,
            _choreRepositoryMock.Object,
            _choreMessageRepositoryMock.Object,
            _userRepositoryMock.Object,
            _choreEnrollmentRepositoryMock.Object,
            _cacheMock.Object);
    }

    [Fact]
    public async Task GetAllChoresAsync_ShouldReturnListOfChores()
    {
        var colocationId = Guid.NewGuid();
        var cacheKey = $"chores:{colocationId}";
        var chores = new List<ChoreOutput> { new() { Id = Guid.NewGuid(), Title = "Test", CreatedBy = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow, IsDone = false } };

        _cacheMock.Setup(c => c.GetOrAddAsync(cacheKey, It.IsAny<Func<ICacheEntry, Task<List<ChoreOutput>>>>()))
            .ReturnsAsync(chores);

        var result = await _service.GetAllChoresAsync(colocationId);

        result.Should().BeEquivalentTo(chores);
    }

    [Fact]
    public async Task GetChoreAsync_ShouldReturnChore_WhenExists()
    {
        var choreId = Guid.NewGuid();
        var choreList = new List<Chore>
    {
        new Chore
        {
            Id = choreId,
            CreatedBy = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(1),
            Title = "Test",
            Description = "Desc",
            IsDone = false
        }
    }.AsQueryable();

        _choreRepositoryMock.Setup(r => r.Query(true)).Returns(choreList);

        var result = await _service.GetChoreAsync(choreId);

        result.Should().NotBeNull();
        result.Id.Should().Be(choreId);
    }

    [Fact]
    public async Task GetChoreMessageFromChoreAsync_ShouldReturnMessages_WhenExist()
    {
        var choreId = Guid.NewGuid();
        var messages = new List<ChoreMessage>
    {
        new ChoreMessage
        {
            Id = Guid.NewGuid(),
            ChoreId = choreId,
            CreatedBy = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            Content = "message"
        }
    }.AsQueryable();

        _choreMessageRepositoryMock.Setup(r => r.Query(true)).Returns(messages);

        var result = await _service.GetChoreMessageFromChoreAsync(choreId);

        result.Should().NotBeNullOrEmpty();
        result.First().Content.Should().Be("message");
    }

    [Fact]
    public async Task GetChoreFromUser_ShouldReturnChores_WhenExist()
    {
        var userId = Guid.NewGuid();
        var chores = new List<Chore>
    {
        new Chore
        {
            Id = Guid.NewGuid(),
            CreatedBy = userId,
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(1),
            Title = "Title",
            IsDone = false,
            ChoreEnrollments = new List<ChoreEnrollment> { new() { UserId = userId } }
        }
    }.AsQueryable();

        _choreRepositoryMock.Setup(r => r.Query(true)).Returns(chores);

        var result = await _service.GetChoreFromUser(userId);

        result.Should().NotBeNullOrEmpty();
        result.First().Title.Should().Be("Title");
    }

    [Fact]
    public async Task GetUserFromChore_ShouldReturnUsers_WhenExist()
    {
        var choreId = Guid.NewGuid();
        var users = new List<User>
    {
        new User
        {
            Id = Guid.NewGuid(),
            Username = "user1",
            Email = "user@example.com",
            ColocationId = Guid.NewGuid(),
            ChoreEnrollments = new List<ChoreEnrollment> { new() { ChoreId = choreId } }
        }
    }.AsQueryable();

        _userRepositoryMock.Setup(r => r.Query(true)).Returns(users);

        var result = await _service.GetUserFromChore(choreId);

        result.Should().NotBeNullOrEmpty();
        result.First().Username.Should().Be("user1");
    }


    [Fact]
    public async Task AddChoreAsync_ShouldReturnChoreId()
    {
        var generatedId = Guid.NewGuid();
        var input = new ChoreInput
        {
            ColocationId = Guid.NewGuid(),
            CreatedBy = Guid.NewGuid(),
            Title = "Test chore",
            Enrolled = new List<Guid> { Guid.NewGuid() }
        };

        _choreRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Chore>()))
            .ReturnsAsync((Chore chore) => { chore.Id = generatedId; return chore; });

        _choreEnrollmentRepositoryMock
            .Setup(r => r.AddRangeAsync(It.IsAny<IEnumerable<ChoreEnrollment>>()))
            .ReturnsAsync(1);

        _choreRepositoryMock
            .Setup(r => r.SaveChangesAsync());

        var result = await _service.AddChoreAsync(input);

        result.Should().Be(generatedId);
    }

    [Fact]
    public async Task AddChoreMessageAsync_ShouldReturnMessageId()
    {
        var generatedId = Guid.NewGuid();
        var input = new ChoreMessageInput
        {
            ChoreId = Guid.NewGuid(),
            CreatedBy = Guid.NewGuid(),
            Content = "Test"
        };

        _choreMessageRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<ChoreMessage>()))
            .ReturnsAsync((ChoreMessage msg) => { msg.Id = generatedId; return msg; });

        _choreRepositoryMock
            .Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        var result = await _service.AddChoreMessageAsync(input);

        result.Should().Be(generatedId);
    }

    [Fact]
    public async Task EnrollToChore_ShouldReturnChoreId()
    {
        var userId = Guid.NewGuid();
        var choreId = Guid.NewGuid();

        _choreEnrollmentRepositoryMock.Setup(r => r.AddAsync(It.IsAny<ChoreEnrollment>()))
            .ReturnsAsync((ChoreEnrollment e) => e);

        _choreRepositoryMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await _service.EnrollToChore(userId, choreId);

        result.Should().Be(choreId);
    }
}
