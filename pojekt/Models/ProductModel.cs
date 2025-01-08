using System.ComponentModel.DataAnnotations;

namespace pojekt.Models
{
    public class ProductModel
    {
        [Key]
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Quantity { get; set; }
        public decimal Price {  get; set; }

        public ICollection<OrderModel> Orders { get; set; }
    }
}
