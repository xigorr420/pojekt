namespace pojekt.Models
{
    public class OrderModel
    {
        public int Id { get; set; }
        public int UserId {  get; set; }
        public UserModel User { get; set; }
        public int ProductId {  get; set; }
        public ProductModel Product { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status {  get; set; }

    }
}
