namespace SenfoniCini.Api.DTOs.Orders
{
    public class OrderCreateDto
    {
        public string ShippingAddress { get; set; } = null!;
        public string ShippingCity { get; set; } = null!;
        public string ShippingPostalCode { get; set; } = null!;
        public decimal ShippingFee { get; set; }
    }
}
