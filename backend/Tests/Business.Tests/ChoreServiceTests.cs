using Business.Services;
using EntityFramework.Models;
using EntityFramework.Repositories;
using FluentAssertions;
using LazyCache;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Any;
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
