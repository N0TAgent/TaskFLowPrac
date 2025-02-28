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
        // Репозиторий для работы с сущностями TaskItem, обеспечивающий операции CRUD.
        private readonly IRepository<TaskItem> _taskRepository;
        public TaskService(IRepository<TaskItem> taskRepository)
        {
            _taskRepository = taskRepository;
        }
        // Метод для получения задач конкретного пользователя.
        public async Task<IEnumerable<TaskItem>> GetTasksAsync(int userId)
        {
            var tasks = await _taskRepository.GetAllAsync();
            return tasks.Where(t => t.UserId == userId);
        }
        // Метод для получения конкретной задачи по её идентификатору с проверкой принадлежности к пользователю.
        public async Task<TaskItem> GetTaskByIdAsync(int id, int userId)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null || task.UserId != userId)
                throw new Exception("Задача не найдена или доступ запрещён.");
            return task;
        }
        // Метод для создания новой задачи.
        public async Task<TaskItem> CreateTaskAsync(TaskItem task)
        {
            await _taskRepository.AddAsync(task);
            await _taskRepository.SaveAsync();
            return task;
        }
        // Метод для обновления существующей задачи.
        public async Task<TaskItem> UpdateTaskAsync(TaskItem task)
        {
            _taskRepository.Update(task);
            await _taskRepository.SaveAsync();
            return task;
        }
        // Метод для удаления задачи с проверкой доступа по userId.
        public async Task DeleteTaskAsync(int id, int userId)
        {
            var task = await GetTaskByIdAsync(id, userId);
            _taskRepository.Delete(task);
            await _taskRepository.SaveAsync();
        }
        // Метод для получения всех задач без фильтрации по пользователю.
        public async Task<IEnumerable<TaskItem>> GetAllTasksAsync()
        {
            return await _taskRepository.GetAllAsync();
        }
    }
}
