using System.Collections.Generic;
using System.Threading.Tasks;
using TaskFlow.Models;

namespace TaskFlow.Services
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskItem>> GetTasksAsync(int userId);
        Task<TaskItem> GetTaskByIdAsync(int id, int userId);
        Task<TaskItem> CreateTaskAsync(TaskItem task);
        Task<TaskItem> UpdateTaskAsync(TaskItem task);
        Task DeleteTaskAsync(int id, int userId);
        Task<IEnumerable<TaskItem>> GetAllTasksAsync();
    }
}
