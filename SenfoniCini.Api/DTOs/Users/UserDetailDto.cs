using SenfoniCini.Api.DTOs.Orders;

namespace SenfoniCini.Api.DTOs.Users
{
    public class UserDetailDto
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public string Address { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }

        public bool IsDeleted { get; set; }

        public List<OrderGetAllDto> Orders { get; set; }
        
    }
}
