using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfAppPR4.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int IDCategory {  get; set; }
        public string Image {  get; set; }
        public float Rating { get; set; }
        public int Quantity { get; set; }
        public int Count { get; set; }
        public bool IsOver {  get; set; }
        public int Price { get; set; }

    }
}
