using Business.Models.Output;

namespace Business.Jwt
{
    public class UserInfo
    {
        public UserOutput User { get; set; } = new UserOutput();
        public string Jwt { get; set; } = null!;
    }
}
