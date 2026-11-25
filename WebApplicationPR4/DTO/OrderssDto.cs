namespace WebApplicationPR4.DTO
{
    public class OrderssCreateRequest
    {
        public int IdUser { get; set; }
        public int IdProduct { get; set; }
        public int DeliveryTypeId { get; set; } // Соответствует IddeliveryType в БД
        public int? PickupPointId { get; set; } // Соответствует IdpickupPoint в БД
        public string Address { get; set; }
        public int TotalPrice { get; set; }     // В твоей БД это int
        public List<OrderssItemRequest> Items { get; set; }
    }

    public class OrderssItemRequest
    {
        public int IdProduct { get; set; }
        public int Count { get; set; }
    }

    // DTO для отображения списка заказов
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
        public string ProductName { get; set; }
        public int Count { get; set; }
        public int Price { get; set; }
        public int IdProduct { get; set; }
    }
}
