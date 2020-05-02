using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Template.Db;
using Template.Models;
using Template.Server.DependencyInjection;

namespace Template.Server.Authentication {
    public class LogonService : ILogonService {
        private readonly byte[] _secretBytes;


        private readonly IUnitOfWork _unitOfWork;

        public LogonService(IUnitOfWork unitOfWork, LogonServiceDependencies dependencies) {
            _unitOfWork = unitOfWork;
            _secretBytes = Encoding.ASCII.GetBytes(dependencies.SecretKey);
        }

        public JwtUser Authenticate(string username, string password, string deviceToken = null) {
            var user = _unitOfWork.Users.Login(username, password);
            if (user == null)
                return null;

            var result = (JwtUser) user;
            result.Token = CreateToken(result);
            return result;
        }
        public User DecodeToken(HttpRequest request)
            => DecodeToken(request.Headers["Authorization"].ToString());
        
        public User DecodeToken(string token) {
            var jwt = new JwtSecurityToken(token);
            if (jwt.ValidFrom > DateTime.Now || jwt.ValidTo < DateTime.Now)
                return null;

            var id = jwt.Claims.FirstOrDefault(c=>c.Type == "Id")?.Value;
            return _unitOfWork.Users.Get(id);
        }

        /*
        public JwtUser DecodeToken(string token) {
            var jwt = new JwtSecurityToken(token);
            if (jwt.ValidFrom > DateTime.Now || jwt.ValidTo < DateTime.Now)
                return null;

            return new JwtUser(jwt.Claims);
        }
        */

        public string CreateToken(JwtUser user) {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(
                    user.GetClaims().ToArray()
                ),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_secretBytes),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(securityToken);
        }
    }
}