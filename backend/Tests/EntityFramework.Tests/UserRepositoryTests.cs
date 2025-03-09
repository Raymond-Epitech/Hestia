using EntityFramework.Context;
using EntityFramework.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Tests.Hestia;

public class UserRepositoryTests
{
    private readonly DbContextOptions<HestiaContext> _dbContextOptions;

    public UserRepositoryTests()
    {
        _dbContextOptions = new DbContextOptionsBuilder<HestiaContext>()
            .UseInMemoryDatabase(databaseName: "HestiaDbForUnitTests")
            .Options;
    }

    [Fact]
    public async Task GetUserByIdAsync_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        using var context = new HestiaContext(_dbContextOptions);
        var userRepository = new UserRepository(context);

        var userId = Guid.NewGuid();
        var expectedUser = new User
        {
            Id = userId,
            Username = "TestUser",
            Email = "test@example.com",
            CreatedAt = DateTime.UtcNow,
            LastConnection = DateTime.UtcNow,
            PathToProfilePicture = ""
        };

        context.User.Add(expectedUser);
        await context.SaveChangesAsync();

        // Act
        var result = await userRepository.GetUserByIdAsync(userId);

        // Assert
        result.Should().NotBeNull(); 
        result!.Should().BeEquivalentTo(expectedUser);
    }
}
