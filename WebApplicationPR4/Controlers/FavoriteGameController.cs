using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApplicationPR4.DTO;
using WebApplicationPR4.Models;
using WebApplicationPR4.Controlers;
using WebApplicationPR4.Controllers;


namespace WebApplicationPR4.Controlers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FavoriteGameController : ControllerBase
    {
        private readonly PR4Context _context;
        public FavoriteGameController(PR4Context context)
        {
            _context = context;
        }
        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<FavoriteGameDto>>> GetFavorites(int userId)
        {
            try
            {
                // 1. Получаем ID продуктов из избранного
                var favProductIds = await _context.Favorites
                    .Where(f => f.Iduser == userId)
                    .Select(f => f.Idproduct)
                    .ToListAsync();

                if (!favProductIds.Any())
                    return Ok(new List<FavoriteGameDto>());

                // 2. Формируем безопасный SQL-запрос
                var idsString = string.Join(",", favProductIds);

                // ⚠️ ЗАМЕНИТЕ ИМЕНА СТОЛБЦОВ НА РЕАЛЬНЫЕ ИЗ ВАШЕЙ БД!
                var sql = $@"
            SELECT 
                p.Id AS Id, 
                p.Name AS Name,      
                p.Rating AS Rating,  
                p.Price AS Price     
            FROM Product p
            WHERE p.Id IN ({idsString})";

                // 3. Выполняем запрос и читаем результат вручную
                // Это обходит проблему с DbSet и работает с любым DTO
                var result = new List<FavoriteGameDto>();

                await using var command = _context.Database.GetDbConnection().CreateCommand();
                command.CommandText = sql;

                await _context.Database.OpenConnectionAsync();

                await using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    // Читаем Price через GetFieldValue<object>, чтобы избежать ошибки приведения типов
                    // Затем явно конвертируем в decimal
                    var priceValue = reader.IsDBNull(reader.GetOrdinal("Price"))
                        ? 0m
                        : Convert.ToDecimal(reader["Price"]); // reader["Price"] вернет object, Convert.ToDecimal справится и с int, и с decimal

                    result.Add(new FavoriteGameDto
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),

                        Name = reader.IsDBNull(reader.GetOrdinal("Name"))
                            ? "Неизвестно"
                            : reader.GetString(reader.GetOrdinal("Name")),

                        // Rating тоже может быть int в БД, используем безопасное чтение
                        Rating = reader.IsDBNull(reader.GetOrdinal("Rating"))
                            ? 0
                            : Convert.ToDouble(reader["Rating"]),

                        Price = priceValue
                    });
                }

                // 4. Сортируем в порядке избранного
                var sortedResult = favProductIds
                    .Join(result,
                          favId => favId,
                          prod => prod.Id,
                          (favId, prod) => prod)
                    .ToList();

                return Ok(sortedResult);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SQL ERROR: {ex.Message}");
                return StatusCode(500, new { error = ex.Message });
            }
        }
        // Маршрут: DELETE api/FavoriteGame/5/100 (где 5 - userId, 100 - productId)
        [HttpDelete("{userId}/{productId}")]
        public async Task<IActionResult> RemoveFromFavorites(int userId, int productId)
        {
            try
            {
                // Ищем запись строго по обоим ключам
                var favorite = await _context.Favorites
                    .FirstOrDefaultAsync(f => f.Iduser == userId && f.Idproduct == productId);

                if (favorite == null)
                {
                    // Если записи нет, возвращаем 404, а не ошибку сервера
                    return NotFound(new { message = "Товар не найден в избранном этого пользователя" });
                }

                _context.Favorites.Remove(favorite);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Успешно удалено" });
            }
            catch (Exception ex)
            {
                // Логируем реальную ошибку на сервере
                System.Diagnostics.Debug.WriteLine($"Ошибка удаления: {ex.Message}");
                return StatusCode(500, new { message = "Ошибка сервера при удалении", details = ex.Message });
            }
        }
        // Маршрут: POST api/FavoriteGame/{userId}/{productId}
        [HttpPost("{userId}/{productId}")]
        public async Task<IActionResult> AddToFavorites(int userId, int productId)
        {
            try
            {
                // Проверяем, нет ли уже такой записи (чтобы избежать дубликатов)
                var exists = await _context.Favorites
                    .AnyAsync(f => f.Iduser == userId && f.Idproduct == productId);

                if (exists)
                    return Conflict(new { message = "Уже в избранном" });

                // Создаем новую запись
                var favorite = new Favorite
                {
                    Iduser = userId,
                    Idproduct = productId
                };

                _context.Favorites.Add(favorite);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Добавлено" });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка добавления: {ex.Message}");
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
