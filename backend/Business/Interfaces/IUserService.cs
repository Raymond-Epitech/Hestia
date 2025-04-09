using Business.Jwt;
using Shared.Models.Input;
using Shared.Models.Output;
using Shared.Models.Update;

namespace Business.Interfaces
{
    public interface IUserService
    {
        Task<List<UserOutput>> GetAllUserAsync(Guid CollocationId);
        Task<UserOutput> GetUserAsync(Guid id);
        Task<Guid> UpdateUserAsync(UserUpdate user);
        Task<Guid> DeleteUserAsync(Guid id);
        Task<UserInfo> RegisterUserAsync(string googleToken, UserInput userInput);
        Task<UserInfo> LoginUserAsync(string googleToken);
    }
}
