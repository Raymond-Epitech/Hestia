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
        var users = await _context.Users.Where(x => x.ColocationId == CollocationId).Select(x => new UserOutput
        {
            Id = x.Id,
            Username = x.Username,
            Email = x.Email,
            ColocationId = x.ColocationId
        }).ToListAsync();
        return users;
    }

    public async Task<UserOutput?> GetUserOutputByIdAsync(Guid id)
    {
        return await _context.Users.Where(x => x.Id == id).Select(x => new UserOutput
        {
            Id = x.Id,
            Username = x.Username,
            Email = x.Email,
            ColocationId = x.ColocationId
        }).FirstOrDefaultAsync();
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
    }

    public async Task AddBalanceAsync(Balance balance)
    {
        await _context.Balances.AddAsync(balance);
    }

    public async Task<User?> GetUserByIdAsync(Guid id)
    {
        return await _context.Users.FindAsync(id);
    }
    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
    }

    public async Task RemoveAsync(User user)
    {
        _context.Users.Remove(user);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
    }

    public async Task<bool> AnyExistingUserByEmail(string email)
    {
        return await _context.Users.AnyAsync(x => x.Email == email);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
