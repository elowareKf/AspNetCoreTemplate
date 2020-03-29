using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Database.Interfaces {
    public interface IRepository<TModel> {
        TModel Get(string id, bool noTracking = false);
        IQueryable<TModel> Get();
        TModel Update(TModel entity);
        TModel Add(TModel entity);
        void Delete(TModel entity);
        void Delete(string id);
    }
}