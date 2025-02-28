using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Data;
using TaskFlow.Models;

namespace TaskFlow.Repositories
{
    // Репозиторий для работы с сущностями типа User.
    // Реализует интерфейс IRepository<User>, предоставляя стандартные операции CRUD.
    public class UserRepository : IRepository<User>
    {
        // Контекст базы данных для доступа к таблице Users.
        private readonly ApplicationDbContext _context;
        public UserRepository(ApplicationDbContext context) => _context = context;

        public async Task AddAsync(User entity) => await _context.Users.AddAsync(entity);

        public void Delete(User entity) => _context.Users.Remove(entity);

        public async Task<IEnumerable<User>> GetAllAsync() => await _context.Users.ToListAsync();

        public async Task<User> GetByIdAsync(int id) => await _context.Users.FindAsync(id);

        public void Update(User entity) => _context.Users.Update(entity);

        public async Task SaveAsync() => await _context.SaveChangesAsync();
    }
}
