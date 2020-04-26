using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace TaskManagementAPI.Data
{
    public interface IRepository<T> where T : class
    {
        Task<T> GetById(int id);
        Task<IEnumerable<T>> GetAll(int pageIndex = 1, int pageSize = 10);
        Task<IEnumerable<T>> Find(Expression<Func<T, bool>> predicate, int pageIndex = 1, int pageSize = 10);

        void Add(T entity);
        void Remove(T entity);
        void Update(T entity);
    }
}
