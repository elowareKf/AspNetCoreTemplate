using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Template.Db;

namespace Template.Server.Authentication {
    public class LogonService : ILogonService {
        public static string SecretString { get; set; } = null;

        private readonly IUnitOfWork _unitOfWork;
        private readonly byte[] _secretBytes;

        public LogonService(IUnitOfWork unitOfWork) {
            if (SecretString == null)
                throw new InvalidCredentialException();
            _unitOfWork = unitOfWork;
            _secretBytes = Encoding.ASCII.GetBytes(SecretString);
        }

        public JwtUser Authenticate(string username, string password, string deviceToken = null) {
            var user = _unitOfWork.Users.Login(username, password);
            if (user == null)
                return null;

            var result = (JwtUser) user;

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(
                    result.GetClaims().ToArray()
                ),
                Expires = deviceToken != null ? DateTime.Now.AddYears(1) : DateTime.Now.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_secretBytes),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            result.Token = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
            return result;
        }
    }
}