using System.Threading.Tasks;
using TaskFlow.Models;

namespace TaskFlow.Services
{
    public interface IUserService
    {
        Task<User> RegisterAsync(string email, string password);
        Task<string> LoginAsync(string email, string password);
        Task<User> GetByIdAsync(int id);
    }
}
