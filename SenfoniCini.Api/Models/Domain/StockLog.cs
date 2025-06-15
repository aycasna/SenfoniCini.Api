public class StockLog
{
    public int Id { get; set; }

    public int ProductId { get; set; }
    public Product Product { get; set; }

    public string AdminUserId { get; set; }
    public ApplicationUser AdminUser { get; set; }

    public string Action { get; set; }
    public string Remarks { get; set; }
    public DateTime Timestamp { get; set; }
}
