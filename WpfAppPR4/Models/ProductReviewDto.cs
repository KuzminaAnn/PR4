using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfAppPR4.Models
{
    public class ProductReviewDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public double Rating { get; set; }
        public string Text { get; set; }
    }
}
