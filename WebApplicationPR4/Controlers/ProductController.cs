using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationPR4.DTO;
using WebApplicationPR4.Models;

namespace WebApplicationPR4.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly PR4Context _context;

        public ProductController(PR4Context context)
        {
            _context = context;
        }

        // GET: api/product — получить все товары
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _context.Products
                .Include(p => p.IdcategoryNavigation)  // ✅ Правильное имя
                .Select(p => new ProductResponseDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    IdCategory = p.Idcategory,  // ✅ Правильное имя
                    CategoryName = p.IdcategoryNavigation != null ? p.IdcategoryNavigation.Name : null,
                    Image = p.Image,
                    Rating = p.Rating,
                    Quantity = p.Quantity,
                    ProductCount = p.Count,  // ✅ Явное указание свойства
                    IsOver = p.IsOver,
                    Price = p.Price
                })
                .ToListAsync();

            return Ok(products);
        }

        // GET: api/product/5 — получить товар по ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _context.Products
                .Include(p => p.IdcategoryNavigation)  // ✅ Правильное имя
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound(new ProductResponseDto { Success = false, Message = "Товар не найден" });

            return Ok(new ProductResponseDto
            {
                Success = true,
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                IdCategory = product.Idcategory,  // ✅ Правильное имя
                CategoryName = product.IdcategoryNavigation?.Name,
                Image = product.Image,
                Rating = product.Rating,
                Quantity = product.Quantity,
                ProductCount = product.Count,
                IsOver = product.IsOver,
                Price = product.Price
            });
        }

        // POST: api/product — добавить товар
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return BadRequest(new ProductResponseDto { Success = false, Message = "Название обязательно" });

            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                Idcategory = request.IdCategory,  // ✅ Правильное имя
                Image = request.Image,
                Rating = request.Rating,
                Quantity = request.Quantity,
                Count = request.ProductCount,
                IsOver = request.IsOver,
                Price = request.Price
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return Ok(new ProductResponseDto
            {
                Success = true,
                Message = "Товар добавлен",
                Id = product.Id
            });
        }

        // PUT: api/product/5 — редактировать товар
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductRequestDto request)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound(new ProductResponseDto { Success = false, Message = "Товар не найден" });

            product.Name = request.Name;
            product.Description = request.Description;
            product.Idcategory = request.IdCategory;  // ✅ Правильное имя
            product.Image = request.Image;
            product.Rating = request.Rating;
            product.Quantity = request.Quantity;
            product.Count = request.ProductCount;
            product.IsOver = request.IsOver;
            product.Price = request.Price;

            await _context.SaveChangesAsync();

            return Ok(new ProductResponseDto { Success = true, Message = "Товар обновлён" });
        }

        // DELETE: api/product/5 — удалить товар
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound(new ProductResponseDto { Success = false, Message = "Товар не найден" });

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Ok(new ProductResponseDto { Success = true, Message = "Товар удалён" });
        }
    }
}