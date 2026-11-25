using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using WebApplicationPR4.DTO;
using WebApplicationPR4.Models;
namespace WebApplicationPR4.Controlers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HistoryController : ControllerBase
    {
        private readonly PR4Context _context;

        public HistoryController(PR4Context context)
        {
            _context = context;
        }
        [HttpGet("user-orders/{userId}")]
        public async Task<ActionResult<List<OrderHistoryDto>>> GetUserOrdersForList(int userId)
        {
            // Получаем все заказы пользователя
            var orders = await _context.Orders
                .Where(o => o.Iduser == userId)
                .ToListAsync();

            if (!orders.Any()) return Ok(new List<OrderHistoryDto>());

            var orderIds = orders.Select(o => o.Id).ToList();

            // Получаем все позиции товаров из этих заказов
            var positions = await _context.ProductOrders
                .Where(po => orderIds.Contains(po.Idorder))
                .Include(po => po.IdproductNavigation) // Подгружаем название товара
                .ToListAsync();

            // Создаем словарь дат заказов для быстрого доступа
            var dateDict = orders.ToDictionary(o => o.Id, o => o.Data);

            // Формируем плоский список DTO
            var result = positions.Select(po => new OrderHistoryDto
            {
                IdProduct = po.Idproduct,
                Name = po.IdproductNavigation != null ? po.IdproductNavigation.Name : "Товар",
                Count = po.Count,
                Price = po.Price,
                SumPrice = po.SumPrice,
                // Преобразуем DateOnly в DateTime, если нужно
                Data = dateDict.TryGetValue(po.Idorder, out var date) ? date.ToDateTime(TimeOnly.MinValue) : DateTime.MinValue
            }).ToList();

            return Ok(result);
        }
    }
}
