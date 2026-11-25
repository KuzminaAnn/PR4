namespace WebApplicationPR4.DTO
{
    public class OrderHistoryDto
    {
        public string Name { get; set; }
        public int IdProduct { get; set; }
        public int Count { get; set; }  
        public int Price { get; set; }
        public int SumPrice { get; set; }
        public DateTime Data { get; set; }
    }
}
