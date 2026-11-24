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
        public async Task<ActionResult<List<OrderHistoryDto>>> GetUserOrders(int userId)
        {

            var userOrders = await _context.Orders
                .Where(o => o.Iduser == userId)
                .ToListAsync();

            if (!userOrders.Any())
                return Ok(new List<OrderHistoryDto>());

            var orderIds = userOrders.Select(o => o.Id).ToList();

            var positions = await _context.ProductOrders
                .Where(po => orderIds.Contains(po.Idorder))
                .ToListAsync();

            if (!positions.Any())
                return Ok(new List<OrderHistoryDto>());

            var productIds = positions.Select(p => p.Idproduct).Distinct().ToList();

            var productsDict = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id, p => p.Name);

            var ordersDict = userOrders.ToDictionary(o => o.Id, o => o.Data);

            var result = positions.Select(po => new OrderHistoryDto
            {
                Name = productsDict.TryGetValue(po.Idproduct, out var name) ? name : "Товар удален",
                Count = po.Count,
                Price = po.Price,
                SumPrice = po.SumPrice,
                Data = ordersDict.TryGetValue(po.Idorder, out var date) ? date.ToDateTime(TimeOnly.MinValue) : DateTime.MinValue
            }).ToList();

            return Ok(result);
        }
    }
}
