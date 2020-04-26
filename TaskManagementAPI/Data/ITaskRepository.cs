using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TaskManagementAPI.DTOs;
using TaskManagementAPI.Models;

namespace TaskManagementAPI.Data
{
    public interface ITaskRepository : IRepository<UserTask>
    {
        IEnumerable<UserTaskDto> GetUserTasks(int pageIndex = 1, int pageSize = 10);
    }
}
