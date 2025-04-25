using Microsoft.AspNetCore.Identity;

namespace TequlaisRestaurant.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<Order>? Orders { get; set; }
    }
}
