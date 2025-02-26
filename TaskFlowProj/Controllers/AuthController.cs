using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Models;
using TaskFlow.Services;

namespace TaskFlow.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            try
            {
                var user = await _userService.RegisterAsync(model.Email, model.Password);
                return Ok(new { user.Id, user.Email });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            try
            {
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
