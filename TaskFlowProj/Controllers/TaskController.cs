﻿using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using TaskFlow.Models;
using TaskFlow.Services;
using TaskFlowProj.Hubs;

namespace TaskFlow.Controllers
{
    [Route("api/tasks")]
    [ApiController]
    [Authorize]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly IHubContext<NotificationHub> _hubContext;

        public TaskController(ITaskService taskService, IHubContext<NotificationHub> hubContext)
        {
            _taskService = taskService;
            _hubContext = hubContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetTasks()
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var tasks = await _taskService.GetTasksAsync(userId);
            return Ok(tasks);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] TaskItem model)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            model.UserId = userId;
            model.User = null; // Обнуляем вложенный объект пользователя
            var task = await _taskService.CreateTaskAsync(model);
            await _hubContext.Clients.All.SendAsync("TaskCreated", task);
            return Ok(task);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] TaskItem model)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var existingTask = await _taskService.GetTaskByIdAsync(id, userId);
            if (existingTask == null) return NotFound();

            existingTask.Title = model.Title;
            existingTask.Description = model.Description;
            existingTask.Deadline = model.Deadline;
            existingTask.Status = model.Status;

            var updatedTask = await _taskService.UpdateTaskAsync(existingTask);
            await _hubContext.Clients.All.SendAsync("TaskUpdated", updatedTask);
            return Ok(updatedTask);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            await _taskService.DeleteTaskAsync(id, userId);
            await _hubContext.Clients.All.SendAsync("TaskDeleted", id);
            return Ok();
        }
    }
}
