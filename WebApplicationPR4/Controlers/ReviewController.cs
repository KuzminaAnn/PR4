using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationPR4.DTO;
using WebApplicationPR4.Models;

namespace WebApplicationPR4.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly PR4Context _context;

        public ReviewController(PR4Context context)
        {
            _context = context;
        }

        // GET: api/review — все отзывы (для админа)
        // GET: api/review — все отзывы (для админа)
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            // 1. Получаем все отзывы из БД (без проекции!)
            var reviews = await _context.Reviews.ToListAsync();

            // 2. Собираем уникальные ID пользователей и товаров
            var userIds = reviews.Select(r => r.Iduser).Distinct().ToList();
            var productIds = reviews.Select(r => r.Idproduct).Distinct().ToList();

            // 3. Делаем словари для быстрого поиска имён
            var users = await _context.Users
                .Where(u => userIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u.Name);

            var products = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id, p => p.Name);

            // 4. Формируем ответ в памяти (LINQ to Objects, не SQL!)
            var result = reviews.Select(r => new ReviewResponseDto
            {
                Success = true,
                Id = r.Id,
                UserId = r.Iduser,
                UserName = users.ContainsKey(r.Iduser) ? users[r.Iduser] : "Неизвестный",
                ProductId = r.Idproduct,
                ProductName = products.ContainsKey(r.Idproduct) ? products[r.Idproduct] : "Неизвестный",
                Rating = r.Rating,
                Description = r.Description
            }).ToList();

            return Ok(result);
        }

        // DELETE: api/review/5 — удалить некорректный отзыв
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
                return NotFound(new ReviewResponseDto { Success = false, Message = "Отзыв не найден" });

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            return Ok(new ReviewResponseDto { Success = true, Message = "Отзыв удалён" });
        }
        [HttpPost("add")]
        public async Task<IActionResult> AddReview([FromBody] WebApplicationPR4.DTO.Review request)
        {
            if (request == null) return BadRequest();

            // Создаем новую запись в таблице Reviews
            var review = new WebApplicationPR4.Models.Review // Убедись, что класс называется так же, как твоя таблица в БД
            {
                Iduser = request.Iduser,
                Idproduct = request.Idproduct,
                Description = request.Description,
                Rating = request.Rating
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Отзыв добавлен" });
        }
    }
}