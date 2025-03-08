using EntityFramework.Models;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid CollocationId);
    Task<List<User>> GetAllAsync();
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(int id);
}
