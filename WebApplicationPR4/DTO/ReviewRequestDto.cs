namespace WebApplicationPR4.DTO
{
    public class ReviewRequestDto
    {
        public int IdUser { get; set; }
        public int IdProduct { get; set; }
        public string Text { get; set; } // Это пойдет в Description
        public int Rating { get; set; }
    }
}
