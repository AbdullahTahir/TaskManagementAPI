using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskManagementAPI.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _dataContext;

        public UnitOfWork(DataContext dataContext)
        {
            _dataContext = dataContext;
            Users = new UserRepository(_dataContext);
            Tasks = new TaskRepository(_dataContext);
        }
        public IUserRepository Users { get; private set; }

        public ITaskRepository Tasks { get; private set; }

        public async Task<int> Complete()
        {
            return await _dataContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            _dataContext.Dispose();
        }
    }
}
