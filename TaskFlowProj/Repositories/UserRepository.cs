using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Data;
using TaskFlow.Models;

namespace TaskFlow.Repositories
{
    public class UserRepository : IRepository<User>
    {
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
