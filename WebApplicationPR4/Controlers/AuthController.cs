using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationPR4.DTO;
using WebApplicationPR4.Models; // namespace, где лежит PR4Context и User

namespace WebApplicationPR4.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly PR4Context _context;

        // Внедряем сгенерированный контекст через конструктор
        public AuthController(PR4Context context)
        {
            _context = context;
        }

        // GET: api/auth/login?login=...&password=...
        [HttpGet("login")]
        public async Task<IActionResult> Login([FromQuery] LoginRequestDTO request)
        {
            if (string.IsNullOrWhiteSpace(request.Login) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new AuthResponseDto
                {
                    Success = false,
                    Message = "Введите логин и пароль"
                });
            }

            // Ищем пользователя в таблице Users по Login и Password
            // EF Core сам переведёт это в SQL-запрос
            var user = await _context.Users.FirstOrDefaultAsync(u =>u.Login.Trim() == request.Login.Trim() && u.Password.Trim() == request.Password.Trim());

            if (user == null)
            {
                return Unauthorized(new AuthResponseDto
                {
                    Success = false,
                    Message = "Неверный логин или пароль"
                });
            }

            return Ok(new AuthResponseDto
            {
                Success = true,
                Message = "Авторизация успешна",
                UserId = user.Id,
                UserName = user.Name
            });
        }
    }
}