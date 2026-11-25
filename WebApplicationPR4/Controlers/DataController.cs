using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationPR4.DTO;
using WebApplicationPR4.Models;

namespace WebApplicationPR4.Controlers
{
    [ApiController]
    [Route("api/products")]
    public class DataController : ControllerBase
    {
        private readonly PR4Context _context;
        public DataController(PR4Context context)
        {
            _context = context;
        }

        // GET: api/data/getitems
        [HttpGet("getitems")]
        public async Task<ActionResult<List<Product>>> GetItems()
        {
            var products = await _context.Products.Where(p => p.Quantity > 0 && !p.IsOver).ToListAsync();
            return Ok(products);
        }
        [HttpGet("categories")]
        public async Task<ActionResult<List<Category>>> GetCategories()
        {
            var categories = await _context.Categories
                .Select(c => new Category { Id = c.Id, Name = c.Name })
                .ToListAsync();
            return Ok(categories);
        }
        [HttpGet("basket/{userId}")]
        public async Task<ActionResult<List<BasketItemDto>>> GetUserBasket(int userId)
        {
            var basketItems = await _context.Baskets
                .Where(b => b.Iduser == userId)
                .Select(b => new BasketItemDto
                {
                    Id = b.Id,
                    IdUser = b.Iduser,
                    IdProduct = b.Idproduct,
                    ProductName = b.IdproductNavigation.Name,  // Подтягиваем название из Products
                    Price = b.IdproductNavigation.Price,       // Цену за штуку
                    Count = b.Count,
                    SumPrice = b.SumPrice
                })
                .ToListAsync();

            return Ok(basketItems);
        }
        // Удаление товара из корзины
        [HttpDelete("basket/{basketId}")]
        public async Task<IActionResult> RemoveFromBasket(int basketId)
        {
            var item = await _context.Baskets.FindAsync(basketId);
            if (item == null) return NotFound();

            _context.Baskets.Remove(item);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Товар удалён из корзины" });
        }

        // Изменение количества товара
        [HttpPut("basket/{basketId}/count")]
        public async Task<IActionResult> UpdateBasketCount(int basketId, [FromBody] int newCount)
        {
            var item = await _context.Baskets.FindAsync(basketId);
            if (item == null) return NotFound();

            if (newCount <= 0)
            {
                // Если количество 0 или меньше — удаляем товар
                _context.Baskets.Remove(item);
            }
            else
            {
                // Пересчитываем сумму: SumPrice = Count * Price
                var product = await _context.Products.FindAsync(item.Idproduct);
                if (product != null)
                {
                    item.Count = newCount;
                    item.SumPrice = newCount * product.Price;
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Количество обновлено" });
        }
        [HttpPost("addtobasket")]
        public async Task<IActionResult> AddToBasket([FromBody] AddToBasketRequest request)
        {
            if (request == null)
                return BadRequest("Пустой запрос");

            // Проверяем, существует ли товар
            var product = await _context.Products.FindAsync(request.IdProduct);
            if (product == null)
                return NotFound("Товар не найден");

            // Проверяем, есть ли уже этот товар в корзине у пользователя
            var existingItem = await _context.Baskets
                .FirstOrDefaultAsync(b => b.Iduser == request.IdUser && b.Idproduct == request.IdProduct);

            if (existingItem != null)
            {
                // Если товар уже есть — увеличиваем количество и пересчитываем сумму
                existingItem.Count += request.Count;
                existingItem.SumPrice = existingItem.Count * (int)product.Price;
            }
            else
            {
                // Создаём новую запись в корзине
                var basketItem = new Basket
                {
                    Iduser = request.IdUser,
                    Idproduct = request.IdProduct,
                    Count = request.Count,
                    SumPrice = request.Count * (int)product.Price
                };
                _context.Baskets.Add(basketItem);
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Товар добавлен в корзину" });
        }
        //отображение отзывов
        [HttpPost("reviews")]
        public async Task<IActionResult> CreateReview([FromBody] ReviewRequest request) // ✅ Используем DTO из тела запроса
        {
            if (request == null)
                return BadRequest("Пустой запрос");

            // ✅ Берем ID товара ИЗ объекта request, а не из несуществующей переменной productId
            int productId = request.Idproduct;

            // Проверка на валидность ID
            if (productId <= 0)
                return BadRequest("Некорректный ID товара");

            // Ищем товар в базе
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                return NotFound($"Товар с ID {productId} не найден");

            // Создаем отзыв
            var review = new WebApplicationPR4.Models.Review
            {
                Iduser = request.Iduser,
                Idproduct = productId,
                Rating = request.Rating,
                Description = request.Description
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Отзыв успешно добавлен" });
        }
    }
}
