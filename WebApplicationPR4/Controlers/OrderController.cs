using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationPR4.DTO;
using WebApplicationPR4.Models;

namespace WebApplicationPR4.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly PR4Context _context;

        public OrderController(PR4Context context)
        {
            _context = context;
        }

        // GET: api/order — все заказы (для админа)
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            // 1. Получаем все заказы с навигациями (кроме User)
            var orders = await _context.Orders
                .Include(o => o.IdstatusNavigation)
                .Include(o => o.IddeliveryTypeNavigation)
                .Include(o => o.IdpickupPointNavigation)
                .Include(o => o.ProductOrders)
                    .ThenInclude(po => po.IdproductNavigation)
                .ToListAsync();

            // 2. Получаем словарь пользователей (Id → Name)
            var userIds = orders.Select(o => o.Iduser).Distinct().ToList();
            var users = await _context.Users
                .Where(u => userIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u.Name);

            // 3. Формируем DTO
            var result = orders.Select(o => new OrderResponseDto
            {
                Success = true,
                Id = o.Id,
                UserId = o.Iduser,
                UserName = users.ContainsKey(o.Iduser) ? users[o.Iduser] : null,
                Date = o.Data,
                TotalPrice = o.TotalPrice,
                StatusName = o.IdstatusNavigation?.Name,
                DeliveryTypeName = o.IddeliveryTypeNavigation?.Name,
                Address = o.Address,
                PickupPointId = o.IdpickupPoint,
                PickupPointName = o.IdpickupPointNavigation?.Name,
                Items = o.ProductOrders.Select(po => new OrderItemDto
                {
                    ProductId = po.Idproduct,
                    ProductName = po.IdproductNavigation?.Name,
                    ProductCount = po.Count,   // ✅ теперь нет конфликта
                    Price = po.Price,
                    SumPrice = po.SumPrice
                }).ToList()
            }).ToList();

            return Ok(result);
        }

        // GET: api/order/5 — один заказ с деталями
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _context.Orders
                .Include(o => o.IdstatusNavigation)
                .Include(o => o.IddeliveryTypeNavigation)
                .Include(o => o.IdpickupPointNavigation)
                .Include(o => o.ProductOrders)
                    .ThenInclude(po => po.IdproductNavigation)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound(new OrderResponseDto { Success = false, Message = "Заказ не найден" });

            // Получаем имя пользователя
            var user = await _context.Users.FindAsync(order.Iduser);

            return Ok(new OrderResponseDto
            {
                Success = true,
                Id = order.Id,
                UserId = order.Iduser,
                UserName = user?.Name,
                Date = order.Data,
                TotalPrice = order.TotalPrice,
                StatusName = order.IdstatusNavigation?.Name,
                DeliveryTypeName = order.IddeliveryTypeNavigation?.Name,
                Address = order.Address,
                PickupPointId = order.IdpickupPoint,
                PickupPointName = order.IdpickupPointNavigation?.Name,
                Items = order.ProductOrders.Select(po => new OrderItemDto
                {
                    ProductId = po.Idproduct,
                    ProductName = po.IdproductNavigation?.Name,
                    ProductCount = po.Count,
                    Price = po.Price,
                    SumPrice = po.SumPrice
                }).ToList()
            });
        }
    }
}