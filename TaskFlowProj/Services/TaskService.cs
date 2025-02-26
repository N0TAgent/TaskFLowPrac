using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskFlow.Models;
using TaskFlow.Repositories;

namespace TaskFlow.Services
{
    public class TaskService : ITaskService
    {
        private readonly IRepository<TaskItem> _taskRepository;
        public TaskService(IRepository<TaskItem> taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<IEnumerable<TaskItem>> GetTasksAsync(int userId)
        {
            var tasks = await _taskRepository.GetAllAsync();
            return tasks.Where(t => t.UserId == userId);
        }

        public async Task<TaskItem> GetTaskByIdAsync(int id, int userId)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null || task.UserId != userId)
                throw new Exception("Задача не найдена или доступ запрещён.");
            return task;
        }

        public async Task<TaskItem> CreateTaskAsync(TaskItem task)
        {
            await _taskRepository.AddAsync(task);
            await _taskRepository.SaveAsync();
            return task;
        }

        public async Task<TaskItem> UpdateTaskAsync(TaskItem task)
        {
            _taskRepository.Update(task);
            await _taskRepository.SaveAsync();
            return task;
        }

        public async Task DeleteTaskAsync(int id, int userId)
        {
            var task = await GetTaskByIdAsync(id, userId);
            _taskRepository.Delete(task);
            await _taskRepository.SaveAsync();
        }

        public async Task<IEnumerable<TaskItem>> GetAllTasksAsync()
        {
            return await _taskRepository.GetAllAsync();
        }
    }
}
