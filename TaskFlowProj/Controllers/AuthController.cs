using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Models;
using TaskFlow.Services;

namespace TaskFlow.Controllers
{
    // Базовый маршрут для действий контроллера
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        // Поле для доступа к сервису пользователей, через который будут выполняться операции регистрации и аутентификации
        private readonly IUserService _userService;

        // Конструктор, внедряющий зависимость IUserService
        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        // Метод для регистрации нового пользователя
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            try
            {
                // Вызываем асинхронный метод регистрации
                var user = await _userService.RegisterAsync(model.Email, model.Password);
                return Ok(new { user.Id, user.Email });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Метод для аутентификации входа пользователя
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            try
            {
                // Вызываем асинхронный метод аутентификации
                var token = await _userService.LoginAsync(model.Email, model.Password);
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
