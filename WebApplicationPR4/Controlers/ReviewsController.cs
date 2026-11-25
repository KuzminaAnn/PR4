
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationPR4.Models;
using System;

namespace WebApplicationPR4.Controlers
{
    [ApiController]
    [Route("api/[controller]")] // Путь будет /api/reviews
    public class ReviewsController : ControllerBase
    {
        private readonly PR4Context _context;

        public ReviewsController(PR4Context context)
        {
            _context = context;
        }

        [HttpPost("add-review")]
        public async Task<IActionResult> AddReview([FromBody] ReviewRequestDto request)
        {
            if (request == null)
                return BadRequest("Данные пусты");

            try
            {
                // Проверка существования пользователя и товара
                var userExists = await _context.Users.AnyAsync(u => u.Id == request.IdUser);
                var productExists = await _context.Products.AnyAsync(p => p.Id == request.IdProduct);

                if (!userExists || !productExists)
                    return BadRequest("Пользователь или товар не найдены.");

                var review = new Review
                {
                    Iduser = request.IdUser,
                    Idproduct = request.IdProduct,
                    Description = request.Text ?? "", // Заполняем описание текстом отзыва
                    Rating = request.Rating,
                    IsFreezing = false, // По умолчанию отзыв не заморожен
                    UserName = null,    // Эти поля обычно заполняются через навигацию, но если они в таблице:
                    ProductName = null  // Лучше оставить их null, если они не используются для хранения
                };

                _context.Reviews.Add(review);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Отзыв успешно добавлен" });
            }
            catch (Exception ex)
            {
                string errorMsg = ex.Message;
                if (ex.InnerException != null)
                    errorMsg += $" | Inner: {ex.InnerException.Message}";

                return StatusCode(500, errorMsg);
            }
        }
    }

    public class ReviewRequestDto
    {
        public int Id { get; set; }
        public int IdUser { get; set; }
        public int IdProduct { get; set; }
        public string Text { get; set; }
        public int Rating { get; set; }
    }
}