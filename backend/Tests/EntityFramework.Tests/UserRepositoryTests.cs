using EntityFramework.Context;
using EntityFramework.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Shared.Models.Output;

namespace Tests.Hestia.Repository;

public class UserRepositoryTests
{
    private readonly DbContextOptions<HestiaContext> _dbContextOptions;

    public UserRepositoryTests()
    {
        _dbContextOptions = new DbContextOptionsBuilder<HestiaContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public async Task GetAllUserOutputAsync_ShouldReturnAllUsersForColocation()
    {
        // Arrange
        var colocationId = Guid.NewGuid();
        var users = new List<User>
        {
            new User
            {
                Id = Guid.NewGuid(),
                Username = "User1",
                Email = "user1@example.com",
                CreatedAt = DateTime.UtcNow,
                LastConnection = DateTime.UtcNow,
                ColocationId = colocationId,
                PathToProfilePicture = "default.jpg"
            },
            new User
            {
                Id = Guid.NewGuid(),
                Username = "User2",
                Email = "user2@example.com",
                CreatedAt = DateTime.UtcNow,
                LastConnection = DateTime.UtcNow,
                ColocationId = colocationId,
                PathToProfilePicture = "default.jpg",
            }
        };

        using (var context = new HestiaContext(_dbContextOptions))
        {
            context.Users.AddRange(users);
            await context.SaveChangesAsync();
        }

        // Act
        List<UserOutput>? result;
        using (var context = new HestiaContext(_dbContextOptions))
        {
            var repository = new UserRepository(context);
            result = await repository.GetAllUserOutputAsync(colocationId);
        }

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result!.Should().BeEquivalentTo(users.Select(u => new UserOutput
        {
            Id = u.Id,
            Username = u.Username,
            Email = u.Email,
            ColocationId = u.ColocationId
        }));
    }


    [Fact]
    public async Task GetUserByIdAsync_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var expectedUser = new User
        {
            Id = userId,
            Username = "TestUser",
            Email = "test@example.com",
            CreatedAt = DateTime.UtcNow,
            LastConnection = DateTime.UtcNow,
            PathToProfilePicture = "",
            ColocationId = null
        };

        using (var arrangeContext = new HestiaContext(_dbContextOptions))
        {
            arrangeContext.Users.Add(expectedUser);
            await arrangeContext.SaveChangesAsync();
        }

        // Act
        User? result;
        using (var actContext = new HestiaContext(_dbContextOptions))
        {
            var userRepository = new UserRepository(actContext);
            result = await userRepository.GetUserByIdAsync(userId);
        }

        // Assert
        result.Should().NotBeNull();
        result!.Should().BeEquivalentTo(expectedUser, options =>
            options.Excluding(u => u.CreatedAt)
                    .Excluding(u => u.LastConnection));
    }

    [Fact]
    public async Task AddAsync_ShouldAddUser()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "NewUser",
            Email = "newuser@example.com",
            CreatedAt = DateTime.UtcNow,
            LastConnection = DateTime.UtcNow,
            PathToProfilePicture = "",
            ColocationId = null
        };

        using (var context = new HestiaContext(_dbContextOptions))
        {
            var repository = new UserRepository(context);
            await repository.AddAsync(user);
            await repository.SaveChangesAsync();
        }

        // Act
        User? result;
        using (var context = new HestiaContext(_dbContextOptions))
        {
            result = await context.Users.FindAsync(user.Id);
        }

        // Assert
        result.Should().NotBeNull();
        result!.Should().BeEquivalentTo(user, options =>
            options.Excluding(u => u.CreatedAt)
                    .Excluding(u => u.LastConnection));
    }

    [Fact]
    public async Task GetUserByIdAsync_ShouldReturnUser_WhenExists()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "ExistingUser",
            Email = "existing@example.com",
            CreatedAt = DateTime.UtcNow,
            LastConnection = DateTime.UtcNow,
            PathToProfilePicture = "",
            ColocationId = null
        };

        using (var context = new HestiaContext(_dbContextOptions))
        {
            context.Users.Add(user);
            await context.SaveChangesAsync();
        }

        // Act
        User? result;
        using (var context = new HestiaContext(_dbContextOptions))
        {
            var repository = new UserRepository(context);
            result = await repository.GetUserByIdAsync(user.Id);
        }

        // Assert
        result.Should().NotBeNull();
        result!.Should().BeEquivalentTo(user, options =>
            options.Excluding(u => u.CreatedAt)
                    .Excluding(u => u.LastConnection));
    }

    [Fact]
    public async Task UpdateAsync_ShouldModifyUser()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "OldName",
            Email = "user@example.com",
            CreatedAt = DateTime.UtcNow,
            LastConnection = DateTime.UtcNow,
            PathToProfilePicture = "",
            ColocationId = null
        };

        using (var context = new HestiaContext(_dbContextOptions))
        {
            context.Users.Add(user);
            await context.SaveChangesAsync();
        }

        using (var context = new HestiaContext(_dbContextOptions))
        {
            var repository = new UserRepository(context);
            user.Username = "NewName";
            await repository.UpdateAsync(user);
            await repository.SaveChangesAsync();
        }

        // Act
        User? result;
        using (var context = new HestiaContext(_dbContextOptions))
        {
            result = await context.Users.FindAsync(user.Id);
        }

        // Assert
        result.Should().NotBeNull();
        result!.Username.Should().Be("NewName");
    }

    [Fact]
    public async Task RemoveAsync_ShouldDeleteUser()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "ToDelete",
            Email = "delete@example.com",
            CreatedAt = DateTime.UtcNow,
            LastConnection = DateTime.UtcNow,
            PathToProfilePicture = "",
            ColocationId = null
        };

        using (var context = new HestiaContext(_dbContextOptions))
        {
            context.Users.Add(user);
            await context.SaveChangesAsync();
        }

        using (var context = new HestiaContext(_dbContextOptions))
        {
            var repository = new UserRepository(context);
            await repository.RemoveAsync(user);
            await repository.SaveChangesAsync();
        }

        // Act
        User? result;
        using (var context = new HestiaContext(_dbContextOptions))
        {
            result = await context.Users.FindAsync(user.Id);
        }

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetUserByEmailAsync_ShouldReturnUser_WhenExists()
    {
        // Arrange
        var email = "test@example.com";
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "TestUser",
            Email = email,
            CreatedAt = DateTime.UtcNow,
            LastConnection = DateTime.UtcNow,
            PathToProfilePicture = "",
            ColocationId = null
        };

        using (var context = new HestiaContext(_dbContextOptions))
        {
            context.Users.Add(user);
            await context.SaveChangesAsync();
        }

        // Act
        User? result;
        using (var context = new HestiaContext(_dbContextOptions))
        {
            var repository = new UserRepository(context);
            result = await repository.GetUserByEmailAsync(email);
        }

        // Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be(email);
    }

    [Fact]
    public async Task AnyExistingUserByEmail_ShouldReturnTrue_WhenUserExists()
    {
        // Arrange
        var email = "exists@example.com";
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "Exists",
            Email = email,
            CreatedAt = DateTime.UtcNow,
            LastConnection = DateTime.UtcNow,
            PathToProfilePicture = "",
            ColocationId = null
        };

        using (var context = new HestiaContext(_dbContextOptions))
        {
            context.Users.Add(user);
            await context.SaveChangesAsync();
        }

        // Act
        bool exists;
        using (var context = new HestiaContext(_dbContextOptions))
        {
            var repository = new UserRepository(context);
            exists = await repository.AnyExistingUserByEmail(email);
        }

        // Assert
        exists.Should().BeTrue();
    }
}
