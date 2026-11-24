using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfAppPR4.Models
{
    public class HistoryOrder
    {
        public string Name { get; set; }
        public int Count { get; set; }
        public int Price { get; set; }
        public int SumPrice { get; set; }
        public DateTime Data { get; set; }
    }
}
