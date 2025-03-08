using EntityFramework.Models;
using Microsoft.EntityFrameworkCore;
using System;

public class UserRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly UserRepository _repository;

    public UserRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

        _context = new AppDbContext(options);
        _repository = new UserRepository(_context);

        // Seed Data
        _context.Users.Add(new User { Id = 1, Name = "TestUser" });
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnUser_WhenUserExists()
    {
        var user = await _repository.GetByIdAsync(1);
        user.Should().NotBeNull();
        user.Name.Should().Be("TestUser");
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
