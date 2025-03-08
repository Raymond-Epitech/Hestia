using EntityFramework.Models;
using Shared.Models.Output;

public interface IUserRepository
{
    Task<UserOutput?> GetByIdAsync(Guid id);
    Task<List<UserOutput>> GetAllAsync(Guid CollocationId);
    Task AddAsync(User user);
    Task<User?> GetUserByIdAsync(Guid id);
    Task UpdateAsync(User user);
    Task RemoveAsync(User user);
    Task<User> GetUserByEmailAsync(string email);
    Task<bool> AnyExistingUserByEmail(string email);
    Task SaveChangesAsync();
}
