namespace SenfoniCini.Api.DTOs.Orders
{
    public class OrderDetailDto
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string UserId { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string UserEmail { get; set; }
        public string UserPhoneNumber { get; set; }
        public string ShippingAddress { get; set; }
        public string ShippingCity { get; set; }

        
        public string ShippingPostalCode { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal ShippingFee { get; set; }
        public string Status { get; set; }

        public List<OrderItemDto> OrderItems { get; set; }

    }

    public class OrderItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImageUrl { get; set; }
        public decimal ProductPrice { get; set; }
    }
}
