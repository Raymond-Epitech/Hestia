using Business.Services;
using EntityFramework.Models;
using EntityFramework.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Shared.Exceptions;
using Shared.Models.Output;

namespace Tests.Hestia.Service;

public class ColocationServiceTests
{
    private readonly ColocationService _colocationService;
    private readonly Mock<ILogger<ColocationService>> _loggerMock;
    private readonly Mock<IRepository<Colocation>> _repositoryMock;
    private readonly Mock<IRepository<User>> _userRepositoryMock;

    public ColocationServiceTests()
    {
        _loggerMock = new Mock<ILogger<ColocationService>>();
        _repositoryMock = new Mock<IRepository<Colocation>>();
        _userRepositoryMock = new Mock<IRepository<User>>();

        _colocationService = new ColocationService(
            _loggerMock.Object,
            _repositoryMock.Object,
            _userRepositoryMock.Object
        );
    }

    // GET ALL COLOCATIONS

    [Fact]
    public async Task GetAllColocations_ShouldReturnListOfColocations_WhenColocationsExist()
    {
        // Arrange
        var expectedColocations = new List<ColocationOutput>
        {
            new ColocationOutput
            {
                Id = Guid.NewGuid(),
                Name = "TestColocation",
                Address = "123 Main St"
            }
        };
        _repositoryMock.Setup(repo => repo.GetAllAsTypeAsync(c => new ColocationOutput
        {
            Id = c.Id,
            Name = c.Name,
            Address = c.Address
        })).ReturnsAsync(expectedColocations);

        // Act
        var result = await _colocationService.GetAllColocations();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedColocations);
        _repositoryMock.Verify(repo => repo.GetAllAsTypeAsync(c => new ColocationOutput
        {
            Id = c.Id,
            Name = c.Name,
            Address = c.Address
        }), Times.Once);
    }

    // GET COLOCATION

    [Fact]
    public async Task GetColocation_ShouldReturnColocation_WhenColocationExists()
    {
        // Arrange
        var colocationId = Guid.NewGuid();
        var expectedColocation = new ColocationOutput
        {
            Id = colocationId,
            Name = "TestColocation",
            Address = "123 Main St"
        };
        _repositoryMock.Setup(repo => repo.GetByIdAsTypeAsync(colocationId, c => new ColocationOutput
        {
            Id = c.Id,
            Name = c.Name,
            Address = c.Address
        })).ReturnsAsync(expectedColocation);

        // Act
        var result = await _colocationService.GetColocation(colocationId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedColocation);
        _repositoryMock.Verify(repo => repo.GetByIdAsTypeAsync(colocationId, c => new ColocationOutput
        {
            Id = c.Id,
            Name = c.Name,
            Address = c.Address
        }), Times.Once);
    }

    [Fact]
    public async Task GetColocation_ShouldThrowNotFoundException_WhenColocationDoesNotExist()
    {
        // Arrange
        var colocationId = Guid.NewGuid();
        _repositoryMock.Setup(repo => repo.GetByIdAsTypeAsync(colocationId, c => new ColocationOutput
        {
            Id = c.Id,
            Name = c.Name,
            Address = c.Address
        })).ReturnsAsync((ColocationOutput?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _colocationService.GetColocation(colocationId));
        _repositoryMock.Verify(repo => repo.GetByIdAsTypeAsync(colocationId, c => new ColocationOutput
        {
            Id = c.Id,
            Name = c.Name,
            Address = c.Address
        }), Times.Once);
    }

    // ADD COLOCATION

    /*
    [Fact]
    public async Task AddColocation_ShouldAddColocation_WhenValidDataProvided()
    {
        // Arrange
        var colocationInput = new ColocationInput
        {
            Name = "TestColocation",
            Address = "123 Main St",
            CreatedBy = "Test"
        };
        var colocationId = Guid.NewGuid();
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "Test",
            Email = "Test",
            ColocationId = null
        };

        _colocationRepoMock.Setup(repo => repo.AddColocationAsync(It.IsAny<Colocation>())).Returns(Task.CompletedTask);
        _userRepoMock.Setup(repo => repo.GetUserByIdAsync(user.Id)).ReturnsAsync(user);
        _colocationRepoMock.Setup(repo => repo.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _colocationService.AddColocation(colocationInput, user.Id);

        // Assert
        result.Should().NotBeEmpty();
        _colocationRepoMock.Verify(repo => repo.AddColocationAsync(It.IsAny<Colocation>()), Times.Once);
        _userRepoMock.Verify(repo => repo.GetUserByIdAsync(user.Id), Times.Once);
        _colocationRepoMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task AddColocation_ShouldThrowNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange
        var colocationInput = new ColocationInput
        {
            Name = "TestColocation",
            Address = "123 Main St",
            CreatedBy = "Test"
        };
        var userId = Guid.NewGuid();

        _userRepoMock.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _colocationService.AddColocation(colocationInput, userId));
    }

    // UPDATE COLOCATION

    [Fact]
    public async Task UpdateColocation_ShouldUpdateColocation_WhenColocationExists()
    {
        // Arrange
        var colocationUpdate = new ColocationUpdate
        {
            Id = Guid.NewGuid(),
            Name = "UpdatedColocation",
            Address = "456 New St"
        };
        var existingColocation = new Colocation
        {
            Id = colocationUpdate.Id,
            Name = "OldName",
            Address = "OldAddress",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "Test"
        };

        _colocationRepoMock.Setup(repo => repo.GetColocationFromIdAsync(colocationUpdate.Id)).ReturnsAsync(existingColocation);
        _colocationRepoMock.Setup(repo => repo.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        await _colocationService.UpdateColocation(colocationUpdate);

        // Assert
        existingColocation.Name.Should().Be(colocationUpdate.Name);
        existingColocation.Address.Should().Be(colocationUpdate.Address);
        _colocationRepoMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateColocation_ShouldThrowNotFoundException_WhenColocationDoesNotExist()
    {
        // Arrange
        var colocationUpdate = new ColocationUpdate
        {
            Id = Guid.NewGuid(),
            Name = "UpdatedColocation",
            Address = "456 New St"
        };
        _colocationRepoMock.Setup(repo => repo.GetColocationFromIdAsync(colocationUpdate.Id)).ReturnsAsync((Colocation?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _colocationService.UpdateColocation(colocationUpdate));
    }

    // REMOVE COLOCATION

    [Fact]
    public async Task DeleteColocation_ShouldRemoveColocation_WhenColocationExists()
    {
        // Arrange
        var colocationId = Guid.NewGuid();
        var existingColocation = new Colocation
        {
            Id = colocationId,
            Name = "TestColocation",
            Address = "123 Main St",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "Test"
        };

        _colocationRepoMock.Setup(repo => repo.GetColocationFromIdAsync(colocationId)).ReturnsAsync(existingColocation);
        _colocationRepoMock.Setup(repo => repo.DeleteColocationAsync(existingColocation)).Returns(Task.CompletedTask);
        _colocationRepoMock.Setup(repo => repo.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        await _colocationService.DeleteColocation(colocationId);

        // Assert
        _colocationRepoMock.Verify(repo => repo.GetColocationFromIdAsync(colocationId), Times.Once);
        _colocationRepoMock.Verify(repo => repo.DeleteColocationAsync(It.Is<Colocation>(c => c.Id == colocationId)), Times.Once);
        _colocationRepoMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteColocation_ShouldThrowNotFoundException_WhenColocationDoesNotExist()
    {
        // Arrange
        var colocationId = Guid.NewGuid();
        _colocationRepoMock.Setup(repo => repo.GetColocationFromIdAsync(colocationId)).ReturnsAsync((Colocation?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _colocationService.DeleteColocation(colocationId));

        _colocationRepoMock.Verify(repo => repo.GetColocationFromIdAsync(colocationId), Times.Once);
        _colocationRepoMock.Verify(repo => repo.DeleteColocationAsync(It.IsAny<Colocation>()), Times.Never);
        _colocationRepoMock.Verify(repo => repo.SaveChangesAsync(), Times.Never);
    }
    */
}
