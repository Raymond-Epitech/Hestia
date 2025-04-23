using Business.Jwt;
using Shared.Models.DTO;
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
        Task<Guid> QuitColocationAsync(Guid id);
        Task<UserInfo> RegisterUserAsync(string code, UserInput userInput, GoogleCredentials googleCredentials);
        Task<UserInfo> LoginUserAsync(string code, GoogleCredentials googleCredentials);
    }
}
