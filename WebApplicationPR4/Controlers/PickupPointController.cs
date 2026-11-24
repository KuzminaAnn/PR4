using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationPR4.DTO;
using WebApplicationPR4.Models;

namespace WebApplicationPR4.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PickupPointController : ControllerBase
    {
        private readonly PR4Context _context;

        public PickupPointController(PR4Context context)
        {
            _context = context;
        }

        // GET: api/pickuppoint — все пункты выдачи
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var points = await _context.PickupPoints
                .Select(p => new PickupPointDto
                {
                    Success = true,
                    Id = p.Id,
                    Name = p.Name,
                })
                .ToListAsync();

            return Ok(points);
        }

        // GET: api/pickuppoint/5 — один пункт выдачи
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var point = await _context.PickupPoints.FindAsync(id);
            if (point == null)
                return NotFound(new PickupPointDto { Success = false, Message = "Пункт выдачи не найден" });

            return Ok(new PickupPointDto
            {
                Success = true,
                Id = point.Id,
                Name = point.Name,
            });
        }

        // POST: api/pickuppoint — добавить пункт выдачи
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PickupPointDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return BadRequest(new PickupPointDto { Success = false, Message = "Название и адрес обязательны" });

            var point = new PickupPoint
            {
                Name = request.Name,
            };

            _context.PickupPoints.Add(point);
            await _context.SaveChangesAsync();

            return Ok(new PickupPointDto
            {
                Success = true,
                Message = "Пункт выдачи добавлен",
                Id = point.Id
            });
        }

        // PUT: api/pickuppoint/5 — редактировать пункт выдачи
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PickupPointDto request)
        {
            var point = await _context.PickupPoints.FindAsync(id);
            if (point == null)
                return NotFound(new PickupPointDto { Success = false, Message = "Пункт выдачи не найден" });

            point.Name = request.Name;

            await _context.SaveChangesAsync();

            return Ok(new PickupPointDto { Success = true, Message = "Пункт выдачи обновлён" });
        }

        // DELETE: api/pickuppoint/5 — удалить пункт выдачи
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var point = await _context.PickupPoints.FindAsync(id);
            if (point == null)
                return NotFound(new PickupPointDto { Success = false, Message = "Пункт выдачи не найден" });

            // Проверка: есть ли заказы с этим пунктом выдачи
            var hasOrders = await _context.Orders.AnyAsync(o => o.IdpickupPoint == id);
            if (hasOrders)
                return Conflict(new PickupPointDto { Success = false, Message = "Нельзя удалить: есть заказы с этим пунктом выдачи" });

            _context.PickupPoints.Remove(point);
            await _context.SaveChangesAsync();

            return Ok(new PickupPointDto { Success = true, Message = "Пункт выдачи удалён" });
        }
    }
}