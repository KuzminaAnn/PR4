using System.Collections.Generic;

namespace WebApplicationPR4.DTO
{
    public class OrderItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int ProductCount{ get; set; }
        public int Price { get; set; }
        public int SumPrice { get; set; }
    }

    public class OrderResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int? Id { get; set; }
        public int? UserId { get; set; }
        public string? UserName { get; set; }
        public DateOnly? Date { get; set; } 
        public int? TotalPrice { get; set; }
        public int? IdStatus { get; set; }
        public string? StatusName { get; set; }
        public string? DeliveryTypeName { get; set; }
        public string? Address { get; set; }
        public int? PickupPointId { get; set; }
        public string? PickupPointName { get; set; }
        public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
    }
    public class ChangeStatusDto
    {
        public int IdStatus { get; set; }
    }
}