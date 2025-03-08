using Business.Jwt;
using Shared.Models.Input;
using Shared.Models.Output;
using Shared.Models.Update;

namespace Business.Interfaces
{
    public interface IUserService
    {
        Task<List<UserOutput>> GetAllUser(Guid CollocationId);
        Task<UserOutput> GetUser(Guid id);
        Task UpdateUser(UserUpdate user);
        Task DeleteUser(Guid id);
        Task<UserInfo> RegisterUser(string googleToken, UserInput userInput);
        Task<UserInfo> LoginUser(string googleToken);
    }
}
