using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationPR4.DTO;
using WebApplicationPR4.Models;

namespace WebApplicationPR4.Controlers
{
    [ApiController]
    [Route("api/orderss")] // Используем orderss, чтобы не конфликтовать с существующим api/order
    public class OrderssController : ControllerBase
    {
        private readonly PR4Context _context;

        public OrderssController(PR4Context context)
        {
            _context = context;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateOrderss([FromBody] OrderssCreateRequest request)
        {
            if (request == null || request.Items == null || !request.Items.Any())
                return BadRequest("Пустой заказ");

            // Проверка суммы для доставки (предположим, что ID доставки = 1, а ПВЗ = 2. Проверь свои ID в БД!)
            // if (request.DeliveryTypeId == 1 && request.TotalPrice < 600)
            //     return BadRequest("При заказе менее 600 руб. доступна только доставка в ПВЗ");

            var order = new Order
            {
                Iduser = request.IdUser,
                Data = DateOnly.FromDateTime(DateTime.Now),
                TotalPrice = request.TotalPrice,
                IddeliveryType = request.DeliveryTypeId,
                Address = request.Address ?? "",
                IdpickupPoint = request.PickupPointId,
                Idstatus = 1 // Статус "Новый" (укажи правильный ID из таблицы Status)
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync(); // Сохраняем, чтобы получить order.Id

            // Добавляем товары в заказ
            foreach (var itemReq in request.Items)
            {
                var product = await _context.Products.FindAsync(itemReq.IdProduct);
                if (product == null) continue;

                var orderItem = new ProductOrder // Убедись, что класс называется именно ProductOrder
                {
                    Idorder = order.Id,
                    Idproduct = product.Id,
                    Count = itemReq.Count,
                    Price = (int)(product.Price * itemReq.Count) // Приводим к int, как в твоей БД
                };
                _context.ProductOrders.Add(orderItem);
            }

            await _context.SaveChangesAsync();

            // Очищаем корзину, если заказ оформлен из неё (если товаров > 1)
            if (request.Items.Count > 1)
            {
                var basketItems = await _context.Baskets.Where(b => b.Iduser == request.IdUser).ToListAsync();
                _context.Baskets.RemoveRange(basketItems);
                await _context.SaveChangesAsync();
            }

            return Ok(new { success = true, orderId = order.Id });
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<OrderssDto>>> GetUserOrderss(int userId)
        {
            var orders = await _context.Orders
                .Where(o => o.Iduser == userId)
                .Include(o => o.ProductOrders).ThenInclude(po => po.IdproductNavigation) // Подтягиваем товары
                .Include(o => o.IddeliveryTypeNavigation)
                .Include(o => o.IdpickupPointNavigation)
                .Include(o => o.IdstatusNavigation)
                .OrderByDescending(o => o.Data)
                .Select(o => new OrderssDto
                {
                    Id = o.Id,
                    Data = DateTime.Now,
                    TotalPrice = o.TotalPrice,
                    DeliveryTypeName = o.IddeliveryTypeNavigation != null ? o.IddeliveryTypeNavigation.Name : "",
                    Address = o.Address,
                    PickupPointName = o.IdpickupPointNavigation != null ? o.IdpickupPointNavigation.Name : "",
                    StatusName = o.IdstatusNavigation != null ? o.IdstatusNavigation.Name : "",
                    Items = o.ProductOrders.Select(po => new OrderssItemDto
                    {
                        ProductName = po.IdproductNavigation != null ? po.IdproductNavigation.Name : "Товар",
                        Count = po.Count,
                        Price = po.Price
                    }).ToList()
                }).ToListAsync();

            return Ok(orders);
        }

        [HttpGet("pickup-points")]
        public async Task<ActionResult<List<PickupPoint>>> GetPickupPoints()
        {
            return Ok(await _context.PickupPoints.ToListAsync());
        }
    }
}
