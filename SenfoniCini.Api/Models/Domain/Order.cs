public class Order
{
    public int Id { get; set; }

    public string UserId { get; set; }
    public ApplicationUser User { get; set; }

    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }

    public string ShippingAddress { get; set; }

    public string ShippingCity { get; set; }
    public string ShippingCost { get; set; }
    public string ShippingPostalCode { get; set; }


    public decimal Total { get; set; }

    public string Status { get; set; } 

    public ICollection<OrderItem> OrderItems { get; set; }
}
