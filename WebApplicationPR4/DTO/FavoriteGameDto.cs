using System.ComponentModel.DataAnnotations;

namespace WebApplicationPR4.DTO
{
    public class FavoriteGameDto
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public double Rating { get; set; }
        public decimal Price { get; set; }
    }
}
