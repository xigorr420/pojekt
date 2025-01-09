using System.ComponentModel.DataAnnotations;

namespace pojekt.Models
{
    public class ProductModel
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public string Category { get; set; }

        [Required]
        public string Quantity { get; set; } // Trzymane jako string, ale sprawdzane w walidacji

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }
        public ICollection<OrderModel>? Orders { get; set; }
    }

}
