namespace SenfoniCini.Api.DTOs.Orders
{
    public class OrderGetAllDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string UserFullName => $"{UserFirstName} {UserLastName}";
        public string UserEmail { get; set; }
        public string UserPhoneNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string ShippingAddress { get; set; }
        public string ShippingCity { get; set; }
        public decimal ShippingFee { get; set; }
        public string ShippingPostalCode { get; set; }
        public string Status { get; set; } 
    }
}
