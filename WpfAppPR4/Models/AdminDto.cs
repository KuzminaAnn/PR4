using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfAppPR4.Models
{
    public class CategoryDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int? Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class ProductResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int? Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int? IdCategory { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public float? Rating { get; set; }
        public int? Quantity { get; set; }
        public int? ProductCount { get; set; }
        public bool? IsOver { get; set; }
        public int? Price { get; set; }

        public string AvailabilityText => (IsOver ?? false) ? "Недоступен" : "Доступен";
    }

    public class ProductRequestDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int IdCategory { get; set; }
        public string Image { get; set; } = string.Empty;
        public float Rating { get; set; }
        public int Quantity { get; set; }
        public int ProductCount { get; set; }
        public bool IsOver { get; set; }
        public int Price { get; set; }
    }
    public class OrderItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int ProductCount { get; set; }
        public int Price { get; set; }
        public int SumPrice { get; set; }
    }
    public class OrderResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int? Id { get; set; }
        public int? UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public int? TotalPrice { get; set; }
        public int? IdStatus { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public string DeliveryTypeName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public int? PickupPointId { get; set; }
        public string PickupPointName { get; set; } = string.Empty;
        public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
    }
    public class ReviewResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public float Rating { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
