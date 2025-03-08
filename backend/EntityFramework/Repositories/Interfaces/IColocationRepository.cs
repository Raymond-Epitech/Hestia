using EntityFramework.Models;
using Shared.Models.Output;

namespace EntityFramework.Repositories.Interfaces
{
    public interface IColocationRepository
    {
        Task<List<ColocationOutput>> GetAllColocationOutputAsync();
        Task<ColocationOutput?> GetColocationOutputFromIdAsync(Guid colocationId);
        Task<Colocation?> GetColocationFromIdAsync(Guid colocationId);
        Task AddColocationAsync(Colocation colocation);
        Task UpdateColocationAsync(Colocation colocation);
        Task DeleteColocationAsync(Colocation colocation);
        Task SaveChangesAsync();
    }
}
