using Shared.Models.Input;
using Shared.Models.Output;
using Shared.Models.Update;

namespace Business.Interfaces
{
    public interface IColocationService
    {
        Task<List<ColocationOutput>> GetAllColocations();
        Task<ColocationOutput> GetColocation(Guid id);
        Task<Guid> AddColocation(ColocationInput colocation);
        Task<Guid> UpdateColocation(ColocationUpdate colocation);
        Task<Guid> DeleteColocation(Guid id);
    }
}
