namespace WebApplicationPR4.DTO
{
    public class ProductRequestDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int IdCategory { get; set; }
        public string Image { get; set; } = string.Empty;
        public double Rating { get; set; }
        public int Quantity { get; set; }
        public int ProductCount { get; set; }
        public bool IsOver { get; set; }       // true = недоступен, false = доступен
        public int Price { get; set; }
    }
}
