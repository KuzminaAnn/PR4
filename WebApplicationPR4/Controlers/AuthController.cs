using System.Security.Cryptography;
using System.Text;
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

        public static string GetHash(string password)
        {
            using (var hash = SHA1.Create())
            {
                return string.Concat(
                    hash.ComputeHash(Encoding.UTF8.GetBytes(password))
                        .Select(x => x.ToString("X2"))
                );
            }
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
            string hashedPassword = GetHash(request.Password);
            // Ищем пользователя в таблице Users по Login и Password
            // EF Core сам переведёт это в SQL-запрос
            var user = await _context.Users.FirstOrDefaultAsync(u =>u.Login.Trim() == request.Login.Trim() && u.Password.Trim() == hashedPassword.Trim());

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
                UserName = user.Name,
                Idrole = user.Idrole
            });
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Login) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new AuthResponseDto
                {
                    Success = false,
                    Message = "Логин и пароль обязательны"
                });
            }

            // Проверяем, не занят ли логин
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Login == request.Login);

            if (existingUser != null)
            {
                return Conflict(new AuthResponseDto
                {
                    Success = false,
                    Message = "Пользователь с таким логином уже существует"
                });
            }
            string hashedPassword = GetHash(request.Password);
            // Создаём нового пользователя
            var newUser = new User
            {
                Login = request.Login,
                Password = hashedPassword,
                Name = request.Name,
                Idrole = 2  // Роль по умолчанию — 2 (обычный пользователь)
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok(new AuthResponseDto
            {
                Success = true,
                Message = "Регистрация успешна",
                UserId = newUser.Id,
                UserName = newUser.Name,
                Idrole = newUser.Idrole
            });
        }
    }
}