using EntityFramework.Context;
using EntityFramework.Models;
using Microsoft.EntityFrameworkCore;
using Shared.Models.Output;

public class UserRepository : IUserRepository
{
    private readonly HestiaContext _context;

    public UserRepository(HestiaContext context)
    {
        _context = context;
    }

    public async Task<List<UserOutput>> GetAllUserOutputAsync(Guid CollocationId)
    {
        var users = await _context.User.Where(x => x.CollocationId == CollocationId).Select(x => new UserOutput
        {
            Id = x.Id,
            Username = x.Username,
            Email = x.Email
        }).ToListAsync();
        return users;
    }

    public async Task<UserOutput?> GetUserOutputByIdAsync(Guid id)
    {
        return await _context.User.Where(x => x.Id == id).Select(x => new UserOutput
        {
            Id = x.Id,
            Username = x.Username,
            Email = x.Email
        }).FirstOrDefaultAsync();
    }

    public async Task AddAsync(User user)
    {
        await _context.User.AddAsync(user);
    }

    public async Task<User?> GetUserByIdAsync(Guid id)
    {
        return await _context.User.FirstOrDefaultAsync(x => x.Id == id);
    }
    public async Task UpdateAsync(User user)
    {
        _context.User.Update(user);
    }

    public async Task RemoveAsync(User user)
    {
        _context.User.Remove(user);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _context.User.FirstOrDefaultAsync(x => x.Email == email);
    }

    public async Task<bool> AnyExistingUserByEmail(string email)
    {
        return await _context.User.AnyAsync(x => x.Email == email);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
