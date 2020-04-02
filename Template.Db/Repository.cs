using System.Linq;
using Microsoft.EntityFrameworkCore;
using Template.Db.Interfaces;
using Template.Models;

namespace Template.Db {
    public class Repository<TModel> : IRepository<TModel> where TModel : ModelBase {
        protected readonly DbContext _context;
        protected readonly DbSet<TModel> _dbSet;

        public Repository(DbSet<TModel> dbSet, DbContext context) {
            _dbSet = dbSet;
            _context = context;
        }


        /// <summary>
        /// Override this to include necessary Properties
        /// </summary>
        /// <returns></returns>
        protected virtual IQueryable<TModel> GetIncluded() => _dbSet;

        public virtual TModel Get(string id, bool noTracking = false) {
            IQueryable<TModel> set = GetIncluded();
            if (noTracking)
                set = _dbSet.AsNoTracking();
            
            return set.FirstOrDefault(e => e.Id == id);
        }

        public virtual IQueryable<TModel> Get() {
            return _dbSet.Where(e => e.Deleted == null);
        }

        public virtual void Delete(TModel entity) {
            _dbSet.Remove(entity);
            _context.SaveChanges();
        }

        public void Delete(string id) {
            Delete(Get(id));
        }

        public virtual TModel Update(TModel entity) {
            _dbSet.Update(entity);
            _context.SaveChanges();
            return entity;
        }


        public virtual TModel Add(TModel entity) {
            _dbSet.Add(entity);
            _context.SaveChanges();
            return entity;
        }
    }

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