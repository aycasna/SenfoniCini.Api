using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }


    //public ICollection<Order> Orders { get; set; }
    //public ICollection<CartItem> CartItems { get; set; }

}
