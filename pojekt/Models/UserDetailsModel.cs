using System.ComponentModel.DataAnnotations;

namespace pojekt.Models
{
    public class UserDetailsModel
    {
        [Key]
        public int UserId { get; set; }
        public string PhoneNumber { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string HouseNumber { get; set; }

        public UserModel User { get; set; }
    }
}
