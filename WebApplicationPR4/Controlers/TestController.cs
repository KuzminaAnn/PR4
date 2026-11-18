using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationPR4.Models;

namespace WebApplicationPR4.Controlers
{
    [ApiController]
    [Route("api/test")] // Жёстко задаём путь
    public class TestController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetTest()
        {
            return Ok(new { message = "Работает!" });
        }
    }
}
