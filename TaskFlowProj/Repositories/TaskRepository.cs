using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Data;
using TaskFlow.Models;

namespace TaskFlow.Repositories
{
    public class TaskRepository : IRepository<TaskItem>
    {
        private readonly ApplicationDbContext _context;
        public TaskRepository(ApplicationDbContext context) => _context = context;

        public async Task AddAsync(TaskItem entity) => await _context.Tasks.AddAsync(entity);

        public void Delete(TaskItem entity) => _context.Tasks.Remove(entity);

        public async Task<IEnumerable<TaskItem>> GetAllAsync() => await _context.Tasks.ToListAsync();

        public async Task<TaskItem> GetByIdAsync(int id) => await _context.Tasks.FindAsync(id);

        public void Update(TaskItem entity) => _context.Tasks.Update(entity);

        public async Task SaveAsync() => await _context.SaveChangesAsync();
    }
}
