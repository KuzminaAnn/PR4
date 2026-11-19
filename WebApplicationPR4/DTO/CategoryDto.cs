namespace WebApplicationPR4.DTO
{
    public class CategoryDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int? Id { get; set; }
        public string? Name { get; set; }
    }
}
