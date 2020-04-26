using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace TaskManagementAPI.Data
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly DataContext _dataContext;

        public Repository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public void Add(T entity)
        {
            _dataContext.Set<T>().Add(entity);
        }

        public void Remove(T entity)
        {
            _dataContext.Set<T>().Remove(entity);
        }

        public void Update(T entity)
        {
            _dataContext.Set<T>().Update(entity);
        }

        public async Task<IEnumerable<T>> Find(Expression<Func<T, bool>> predicate, int pageIndex = 1, int pageSize = 10)
        {
            return await _dataContext.Set<T>().Where(predicate).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAll(int pageIndex = 1, int pageSize = 10)
        {
            return await _dataContext.Set<T>().Skip((pageIndex - 1)*pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<T> GetById(int id)
        {
            return await _dataContext.Set<T>().FindAsync(id);
        }       
    }
}
