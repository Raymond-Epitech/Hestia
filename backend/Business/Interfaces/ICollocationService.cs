using Business.Models.Input;
using Business.Models.Output;
using Business.Models.Update;

namespace Business.Interfaces
{
    public interface ICollocationService
    {
        Task<List<CollocationOutput>> GetAllCollocations();
        Task<CollocationOutput> GetCollocation(Guid id);
        Task<Guid> AddCollocation(CollocationInput collocation, Guid? AddedBy);
        Task UpdateCollocation(CollocationUpdate collocation);
        Task DeleteCollocation(Guid id);
    }
}
