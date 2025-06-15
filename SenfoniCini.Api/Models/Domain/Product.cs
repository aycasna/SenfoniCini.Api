public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
    public decimal Price { get; set; }

    public bool IsSold { get; set; }
    public bool IsDeleted { get; set; }

    public ICollection<ProductCategory> ProductCategories { get; set; }
    public ICollection<ProductColor> ProductColors { get; set; }

    public ICollection<OrderItem> OrderItems { get; set; }
    public ICollection<CartItem> CartItems { get; set; }

    public ICollection<StockLog> StockLogs { get; set; }
}