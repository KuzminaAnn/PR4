namespace WebApplicationPR4.DTO
{
    public class BasketItemDto
    {
        public int Id { get; set; }
        public int IdUser { get; set; }
        public int IdProduct { get; set; }
        public string ProductName { get; set; }  // Название товара
        public decimal Price { get; set; }       // Цена за штуку
        public int Count { get; set; }           // Количество
        public decimal SumPrice { get; set; }    // Итоговая сумма
    }
}
