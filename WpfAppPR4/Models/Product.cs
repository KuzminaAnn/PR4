using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//алё работать будем сегодня или как???
namespace WpfAppPR4.Models
{
    public class Product
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("image")]
        public string Image { get; set; }

        [JsonProperty("rating")]
        public double Rating { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
        public int Idcategory { get; set; }
    }
}
