namespace WebApplicationPR4.DTO
{
    public class AddToBasketRequest
    {
        public int IdUser { get; set; }
        public int IdProduct { get; set; }
        public int Count { get; set; }
        public int SumPrice { get; set; }
    }
}
