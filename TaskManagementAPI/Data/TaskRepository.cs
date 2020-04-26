using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TaskManagementAPI.DTOs;
using TaskManagementAPI.Models;

namespace TaskManagementAPI.Data
{
    public class TaskRepository : Repository<UserTask> , ITaskRepository
    {
        public TaskRepository(DataContext dataContext):base(dataContext)
        {
        }
        public DataContext DataContext
        {
            get { return _dataContext as DataContext; }
        }
        public IEnumerable<UserTaskDto> GetUserTasks(int pageIndex, int pageSize)
        {
            return (from user in _dataContext.Users
                          from task in _dataContext.UserTasks.Where(m => m.UserId == user.Id).DefaultIfEmpty()
                          select new UserTaskDto
                          {
                              FirstName = user.FirstName,
                              LastName = user.LastName,
                              Role = user.Role,
                              UserId = user.Id,
                              UserName = user.UserName,
                              TaskId = task.Id,
                              TaskTitle = task.Title,
                              TaskDescription = task.Description,
                              TaskStatus = task.Status
                          }).Skip((pageIndex - 1) * pageSize)
                          .Take(pageSize)
                          .ToList();
        }
    }
}
