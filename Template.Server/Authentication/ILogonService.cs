using Microsoft.AspNetCore.Http;
using Template.Models;

namespace Template.Server.Authentication {
    public interface ILogonService {
        JwtUser Authenticate(string username, string password, string deviceToken = null);
        public User DecodeToken(HttpRequest request);
        public User DecodeToken(string token);

    }
}