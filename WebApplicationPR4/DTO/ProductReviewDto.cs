namespace WebApplicationPR4.DTO
{
    public class ProductReviewDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public double Rating { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
    }
}
