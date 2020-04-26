using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManagementAPI.Data;
using TaskManagementAPI.DTOs;
using TaskManagementAPI.Models;

namespace TaskManagementAPI.Controllers
{
    [Authorize(Roles = "1")]
    [Route("api/[controller]")]
    [ApiController]
    public class ManageTaskController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ManageTaskController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpPost("AddUpdate")]
        public async Task<IActionResult> AddUpdate(UserTask userTask)
        {
            try
            {
                if (userTask.Id == 0)
                {
                    _unitOfWork.Tasks.Add(userTask);                    
                }
                else
                {
                    _unitOfWork.Tasks.Update(userTask);
                }

                await _unitOfWork.Complete();
                return Ok("Saved Successfuly");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }
        [HttpPost("Delete")]
        public async Task<IActionResult> Delete(int Id)
        {
            var taskToDeleta =   await _unitOfWork.Tasks.GetById(Id);
            if(taskToDeleta != null)
            {
                _unitOfWork.Tasks.Remove(taskToDeleta);
                await _unitOfWork.Complete();
            }

            return Ok("Task Deleted Successfuly");
        }

        [HttpGet("{id}")]
        public async Task<UserTask> GetTask(int id)
        {
            return await _unitOfWork.Tasks.GetById(id);
        }

        [HttpGet("list")]
        public async Task<IEnumerable<UserTask>> GetTasksList(int pageIndex, int pageSize)
        {
            return await _unitOfWork.Tasks.GetAll(pageIndex, pageSize);
        }

        [HttpGet("GroupByUser")]
        public object GetTasksByUser(int pageIndex = 1, int pageSize = 10)
        {
            var taskGroupByUser =  _unitOfWork.Tasks.GetUserTasks(pageIndex, pageSize);

            var a = taskGroupByUser.GroupBy(m => m.UserName)
                .Select(m=>new 
                 {
                   UserName = m.Key,
                   Tasks = m.ToList().Select(x=>new { x.TaskId,x.TaskTitle,x.TaskDescription,x.TaskStatus})
                 }).ToList();
                
            return a;
        }
    }
   
}
