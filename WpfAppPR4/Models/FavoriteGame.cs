using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfAppPR4.Models
{
    public class FavoriteGame
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Rating { get; set; }
        public decimal Price { get; set; }
    }
}
