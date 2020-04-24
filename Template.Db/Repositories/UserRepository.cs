using System.Linq;
using Microsoft.EntityFrameworkCore;
using Template.Models;

namespace Template.Db {
    public class UserRepository : Repository<User> {
        public User Login(string username, string password) {
            var passwordHash = User.CreateMD5(password);
            return _dbSet.FirstOrDefault(u =>
                u.UserName == username && u.PasswordHash == passwordHash);
        }

        public UserRepository(DbSet<User> dbSet, DbContext context) : base(dbSet, context) {
        }
    }
}