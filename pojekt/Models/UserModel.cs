using System.ComponentModel.DataAnnotations;

namespace pojekt.Models
{
    public class UserModel
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } = "User";

        public ICollection<OrderModel>? Orders { get; set; }
        public UserDetailsModel? UserDetails { get; set; }
    }
}
