using Business.Models.Input;
using Business.Models.Output;

namespace Business.Interfaces
{
    public interface ICollocationService
    {
        Task<List<CollocationOutput>> GetAllCollocations();
        Task<CollocationOutput> GetCollocation(Guid id);
        Task AddCollocation(CollocationInput collocation);
        Task UpdateCollocation(CollocationUpdate collocation);
        Task DeleteCollocation(Guid id);
    }
}
