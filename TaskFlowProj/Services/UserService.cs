using System;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TaskFlow.Models;
using TaskFlow.Repositories;
using TaskFlow.Helpers;
using BCrypt.Net;
using TaskFlowProj.Hubs;

namespace TaskFlow.Services
{
    // Сервис для управления пользователями: регистрация, аутентификация и получение данных пользователя.
    public class UserService : IUserService
    {
        private readonly IRepository<User> _userRepository;
        private readonly JwtSettings _jwtSettings;
        public UserService(IRepository<User> userRepository, IOptions<JwtSettings> jwtOptions)
        {
            _userRepository = userRepository;
            _jwtSettings = jwtOptions.Value;
        }
        // Метод регистрации нового пользователя.
        // Хэширует пароль и сохраняет нового пользователя в базе данных.
        public async Task<User> RegisterAsync(string email, string password)
        {
            // Проверка на существование пользователя
            var existingUser = (await _userRepository.GetAllAsync()).FirstOrDefault(u => u.Email == email);
            if (existingUser != null)
                throw new Exception("Пользователь с таким email уже существует.");

            var user = new User
            {
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
            };
            await _userRepository.AddAsync(user);
            await _userRepository.SaveAsync();
            return user;
        }
        // Метод аутентификации пользователя.
        // Генерирует и возвращает JWT-токен для дальнейшей аутентификации клиента.
        public async Task<string> LoginAsync(string email, string password)
        {
            var user = (await _userRepository.GetAllAsync()).FirstOrDefault(u => u.Email == email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                throw new Exception("Неверные учетные данные.");

            // Генерация JWT-токена
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        // Метод для получения данных пользователя по его идентификатору.
        public async Task<User> GetByIdAsync(int id)
        {
            return await _userRepository.GetByIdAsync(id);
        }
    }
}
