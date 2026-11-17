using Microsoft.AspNetCore.Mvc;

namespace WebApplicationPR4.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        // GET: api/auth/login?login=admin&password=12345
        [HttpGet("login")]
        public IActionResult Login([FromQuery] string login, [FromQuery] string password)
        {
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                return BadRequest(new { Success = false, Message = "Логин и пароль обязательны" });
            }

            // TODO: Замените эту заглушку на реальную проверку через базу данных (например, через Entity Framework Core)
            // В реальном проекте пароли должны сравниваться в захешированном виде!
            if (login == "admin" && password == "12345")
            {
                return Ok(new
                {
                    Success = true,
                    Message = "Авторизация успешна",
                    Token = "fake-jwt-token-for-example"
                });
            }

            return Unauthorized(new { Success = false, Message = "Неверный логин или пароль" });
        }
    }
}