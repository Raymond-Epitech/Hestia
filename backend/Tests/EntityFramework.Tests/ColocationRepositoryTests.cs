using EntityFramework.Context;
using EntityFramework.Models;
using EntityFramework.Repositories.Implementations;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Shared.Models.Output;

namespace Tests.Hestia.Repository;

public class ColocationRepositoryTests
{
    private readonly DbContextOptions<HestiaContext> _dbContextOptions;

    public ColocationRepositoryTests()
    {
        _dbContextOptions = new DbContextOptionsBuilder<HestiaContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Base unique par test
            .Options;
    }

    [Fact]
    public async Task GetAllColocationOutputAsync_ShouldReturnAllColocations()
    {
        // Arrange
        var colocations = new List<Colocation>
            {
                new Colocation { Id = Guid.NewGuid(), Name = "Coloc A", Address = "123 Rue A", CreatedAt = DateTime.UtcNow, CreatedBy = "User1" },
                new Colocation { Id = Guid.NewGuid(), Name = "Coloc B", Address = "456 Rue B", CreatedAt = DateTime.UtcNow, CreatedBy = "User2" }
            };

        using (var context = new HestiaContext(_dbContextOptions))
        {
            context.Colocation.AddRange(colocations);
            await context.SaveChangesAsync();
        }

        // Act
        List<ColocationOutput>? result;
        using (var context = new HestiaContext(_dbContextOptions))
        {
            var repository = new ColocationRepository(context);
            result = await repository.GetAllColocationOutputAsync();
        }

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result!.Should().BeEquivalentTo(colocations.Select(c => new ColocationOutput
        {
            Id = c.Id,
            Name = c.Name,
            Address = c.Address
        }));
    }

    [Fact]
    public async Task GetColocationOutputFromIdAsync_ShouldReturnCorrectColocation()
    {
        // Arrange
        var colocation = new Colocation { Id = Guid.NewGuid(), Name = "Coloc Unique", Address = "789 Rue C", CreatedAt = DateTime.UtcNow, CreatedBy = "User3" };
        var user = new User { Id = Guid.NewGuid(), Username = "Colocataire1", Email = "user@example.com", CreatedAt = DateTime.UtcNow, LastConnection = DateTime.UtcNow, ColocationId = colocation.Id, PathToProfilePicture = "default.jpg" };

        using (var context = new HestiaContext(_dbContextOptions))
        {
            context.Colocation.Add(colocation);
            context.User.Add(user);
            await context.SaveChangesAsync();
        }

        // Act
        ColocationOutput? result;
        using (var context = new HestiaContext(_dbContextOptions))
        {
            var repository = new ColocationRepository(context);
            result = await repository.GetColocationOutputFromIdAsync(colocation.Id);
        }

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(colocation.Id);
        result.Colocataires.Should().Contain(user.Id);
    }

    [Fact]
    public async Task AddColocationAsync_ShouldAddColocation()
    {
        // Arrange
        var colocation = new Colocation { Id = Guid.NewGuid(), Name = "Nouvelle Coloc", Address = "999 Rue D", CreatedAt = DateTime.UtcNow, CreatedBy = "User4" };

        using (var context = new HestiaContext(_dbContextOptions))
        {
            var repository = new ColocationRepository(context);
            await repository.AddColocationAsync(colocation);
            await repository.SaveChangesAsync();
        }

        // Act
        Colocation? result;
        using (var context = new HestiaContext(_dbContextOptions))
        {
            result = await context.Colocation.FindAsync(colocation.Id);
        }

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Nouvelle Coloc");
    }

    [Fact]
    public async Task UpdateColocationAsync_ShouldModifyColocation()
    {
        // Arrange
        var colocation = new Colocation { Id = Guid.NewGuid(), Name = "Coloc Test", Address = "Initial Address", CreatedAt = DateTime.UtcNow, CreatedBy = "User5" };

        using (var context = new HestiaContext(_dbContextOptions))
        {
            context.Colocation.Add(colocation);
            await context.SaveChangesAsync();
        }

        using (var context = new HestiaContext(_dbContextOptions))
        {
            var repository = new ColocationRepository(context);
            colocation.Address = "Updated Address";
            await repository.UpdateColocationAsync(colocation);
            await repository.SaveChangesAsync();
        }

        // Act
        Colocation? result;
        using (var context = new HestiaContext(_dbContextOptions))
        {
            result = await context.Colocation.FindAsync(colocation.Id);
        }

        // Assert
        result.Should().NotBeNull();
        result!.Address.Should().Be("Updated Address");
    }

    [Fact]
    public async Task DeleteColocationAsync_ShouldRemoveColocation()
    {
        // Arrange
        var colocation = new Colocation { Id = Guid.NewGuid(), Name = "Coloc à Supprimer", Address = "456 Rue E", CreatedAt = DateTime.UtcNow, CreatedBy = "User6" };

        using (var context = new HestiaContext(_dbContextOptions))
        {
            context.Colocation.Add(colocation);
            await context.SaveChangesAsync();
        }

        using (var context = new HestiaContext(_dbContextOptions))
        {
            var repository = new ColocationRepository(context);
            await repository.DeleteColocationAsync(colocation);
            await repository.SaveChangesAsync();
        }

        // Act
        Colocation? result;
        using (var context = new HestiaContext(_dbContextOptions))
        {
            result = await context.Colocation.FindAsync(colocation.Id);
        }

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetColocationFromIdAsync_ShouldReturnCorrectColocation()
    {
        // Arrange
        var colocation = new Colocation { Id = Guid.NewGuid(), Name = "Coloc ID Test", Address = "123 Test Rue", CreatedAt = DateTime.UtcNow, CreatedBy = "User7" };

        using (var context = new HestiaContext(_dbContextOptions))
        {
            context.Colocation.Add(colocation);
            await context.SaveChangesAsync();
        }

        // Act
        Colocation? result;
        using (var context = new HestiaContext(_dbContextOptions))
        {
            var repository = new ColocationRepository(context);
            result = await repository.GetColocationFromIdAsync(colocation.Id);
        }

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be(colocation.Name);
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldPersistData()
    {
        // Arrange
        var colocation = new Colocation { Id = Guid.NewGuid(), Name = "Test Save", Address = "999 Rue F", CreatedAt = DateTime.UtcNow, CreatedBy = "User8" };

        using (var context = new HestiaContext(_dbContextOptions))
        {
            var repository = new ColocationRepository(context);
            await repository.AddColocationAsync(colocation);
            await repository.SaveChangesAsync();
        }

        // Act
        Colocation? result;
        using (var context = new HestiaContext(_dbContextOptions))
        {
            result = await context.Colocation.FindAsync(colocation.Id);
        }

        // Assert
        result.Should().NotBeNull();
    }
}
