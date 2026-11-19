namespace WebApplicationPR4.DTO
{
    public class ReviewResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int? Id { get; set; }
        public int? UserId { get; set; }
        public string? UserName { get; set; }
        public int? ProductId { get; set; }
        public string? ProductName { get; set; }
        public double? Rating { get; set; }
        public string? Description { get; set; }
    }
}
