using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfAppPR4.Models
{
    public class CategoryDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int? Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class ProductResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int? Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int? IdCategory { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public float? Rating { get; set; }
        public int? Quantity { get; set; }
        public int? ProductCount { get; set; }
        public bool? IsOver { get; set; }
        public int? Price { get; set; }

        public string AvailabilityText => (IsOver ?? false) ? "Недоступен" : "Доступен";
    }

    public class ProductRequestDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int IdCategory { get; set; }
        public string Image { get; set; } = string.Empty;
        public float Rating { get; set; }
        public int Quantity { get; set; }
        public int ProductCount { get; set; }
        public bool IsOver { get; set; }
        public int Price { get; set; }
    }
}
