namespace Template.Server.Authentication {
    public interface ILogonService {
        JwtUser Authenticate(string username, string password, string deviceToken = null);
    }
}