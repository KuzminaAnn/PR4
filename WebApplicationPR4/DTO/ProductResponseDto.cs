namespace WebApplicationPR4.DTO
{
    public class ProductResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;

        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? IdCategory { get; set; }
        public string? CategoryName { get; set; }
        public string? Image { get; set; }
        public double? Rating { get; set; }
        public int? Quantity { get; set; }
        public int? ProductCount { get; set; }
        public bool? IsOver { get; set; }
        public int? Price { get; set; }
    }
}
