namespace WebApplicationPR4.DTO
{
    public class ReviewRequest
    {
        public int Iduser { get; set; }
        public int Idproduct { get; set; }
        public double Rating { get; set; }
        public string Description { get; set; }
    }
}
