using Business.Models.Input;
using Business.Models.Output;
using Business.Models.Update;

namespace Business.Interfaces
{
    public interface IUserService
    {
        Task<List<UserOutput>> GetAllUser(Guid CollocationId);
        Task<UserOutput> GetUser(Guid id);
        Task AddUser(UserInput user);
        Task UpdateUser(UserUpdate user);
        Task DeleteUser(Guid id);
        Task<string> LoginUser(string googleToken, string clientId);
    }
}
