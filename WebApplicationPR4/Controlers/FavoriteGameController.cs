using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApplicationPR4.DTO;
using WebApplicationPR4.Models;

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
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FavoriteGame>>> GetFavorites()
        {
            // ВАЖНО: Замените 1 на реальный ID пользователя, если он другой
            // Или раскомментируйте строку ниже, если хотите брать из токена (но вы просили без токена)
            // var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            int userId = 7;

            // 1. Проверяем, есть ли вообще записи в таблице Favorites для этого пользователя
            var favIds = await _context.Favorites
                .Where(f => f.Iduser == userId)
                .Select(f => f.Idproduct)
                .ToListAsync();

            if (!favIds.Any())
            {
                // Если записей нет, возвращаем пустой список явно
                return Ok(new List<FavoriteGame>());
            }
            var products = await _context.Products
                .Where(p => favIds.Contains(p.Id))
                .Select(p => new FavoriteGame
                {
                    Id = p.Id,
                    Name = p.Name,
                    Rating = p.Rating,
                    Price = p.Price
                })
                .ToListAsync();

            return Ok(products);
        }
    }
}
