namespace Business.Interfaces
{
    public interface IUserService
    {
        bool LoginUser(string googleToken, string clientId);
    }
}
