using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Template.Models;

namespace Template.Server.Authentication {
    public class JwtUser {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string Token { get; set; }

        public static explicit operator JwtUser(User user) {
            if (user == null)
                return null;
            
            return new JwtUser {
                Username = user.UserName,
                Id = user.Id,
                Name = user.Name,
            };
        }

        public IEnumerable<Claim> GetClaims() {
            foreach (var property in typeof(JwtUser).GetProperties()
                .Where(p => p.PropertyType == typeof(string))) {
                if (property.GetValue(this) != null)
                    yield return new Claim(property.Name, property.GetValue(this).ToString());
            }
        }
    }
}