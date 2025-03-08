using EntityFramework.Context;
using EntityFramework.Models;
using Microsoft.EntityFrameworkCore;

public class UserRepository : IUserRepository
{
    private readonly HestiaContext _context;

    public UserRepository(HestiaContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid CollocationId)
    {
        var users = await _context.User.Where(x => x.CollocationId == CollocationId).Select(x => new UserOutput
        {
            Id = x.Id,
            Username = x.Username,
            Email = x.Email
        }).ToListAsync();
        return users;
    }

    public async Task<List<User>> GetAllAsync()
    {
        return await _context.User.ToListAsync();
    }

    public async Task AddAsync(User user)
    {
        _context.User.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        _context.User.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var user = await _context.User.FindAsync(id);
        if (user != null)
        {
            _context.User.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}
