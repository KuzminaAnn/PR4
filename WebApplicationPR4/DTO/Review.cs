namespace WebApplicationPR4.DTO
{
    public class Review
    {
        public int Id { get; set; }

        public int Iduser { get; set; }

        public int Idproduct { get; set; }

        public double Rating { get; set; }

        public string Description { get; set; }

        public bool IsFreezing { get; set; }
        public string UserName { get; set; } //собственные
        public string ProductName { get; set; } //собственные
    }
}
