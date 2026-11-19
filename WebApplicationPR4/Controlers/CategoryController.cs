using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationPR4.DTO;
using WebApplicationPR4.Models;

namespace WebApplicationPR4.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly PR4Context _context;

        public CategoryController(PR4Context context)
        {
            _context = context;
        }

        // GET: api/category
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _context.Categories
                .Select(c => new CategoryDto
                {
                    Success = true,
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync();

            return Ok(categories);
        }

        // GET: api/category/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return NotFound(new CategoryDto { Success = false, Message = "Категория не найдена" });

            return Ok(new CategoryDto { Success = true, Id = category.Id, Name = category.Name });
        }

        // POST: api/category
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CategoryDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return BadRequest(new CategoryDto { Success = false, Message = "Название обязательно" });

            var exists = await _context.Categories.AnyAsync(c => c.Name == request.Name);
            if (exists)
                return Conflict(new CategoryDto { Success = false, Message = "Такая категория уже есть" });

            var category = new Category { Name = request.Name };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return Ok(new CategoryDto { Success = true, Message = "Категория добавлена", Id = category.Id });
        }

        // PUT: api/category/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CategoryDto request)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return NotFound(new CategoryDto { Success = false, Message = "Категория не найдена" });

            category.Name = request.Name;
            await _context.SaveChangesAsync();

            return Ok(new CategoryDto { Success = true, Message = "Категория обновлена" });
        }

        // DELETE: api/category/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return NotFound(new CategoryDto { Success = false, Message = "Категория не найдена" });

            // Защита: нельзя удалить категорию, в которой есть товары
            var hasProducts = await _context.Products.AnyAsync(p => p.Idcategory == id);
            if (hasProducts)
                return Conflict(new CategoryDto { Success = false, Message = "В категории есть товары — удаление запрещено" });

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return Ok(new CategoryDto { Success = true, Message = "Категория удалена" });
        }
    }
}