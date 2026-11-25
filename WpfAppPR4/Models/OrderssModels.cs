using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfAppPR4.Models
{
    public class OrderssCreateRequest
    {
        public int IdUser { get; set; }
        public int IdProduct { get; set; }
        public int DeliveryTypeId { get; set; }
        public int? PickupPointId { get; set; }
        public string Address { get; set; }
        public int TotalPrice { get; set; }
        public List<OrderssItemRequest> Items { get; set; }
    }

    public class OrderssItemRequest
    {
        public int IdProduct { get; set; }
        public int Count { get; set; }
        public string ProductName { get; set; } // Нужно для отображения на странице оформления
        public int Price { get; set; }
    }

    public class OrderssDto
    {
        public int Id { get; set; }
        public int IdProduct { get; set; }
        public DateTime Data { get; set; }
        public int TotalPrice { get; set; }
        public string DeliveryTypeName { get; set; }
        public string Address { get; set; }
        public string PickupPointName { get; set; }
        public string StatusName { get; set; }
        public List<OrderssItemDto> Items { get; set; }
    }

    public class OrderssItemDto
    {
        public int IdProduct { get; set; }
        public string ProductName { get; set; }
        public int Count { get; set; }
        public int Price { get; set; }
    }

    public class PickupPointDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
