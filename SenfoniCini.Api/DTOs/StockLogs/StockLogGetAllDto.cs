namespace SenfoniCini.Api.DTOs.StockLogs
{
    public class StockLogGetAllDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string AdminUserId { get; set; }
        public string AdminUserName { get; set; }
        public string Action { get; set; }
        public string Remarks { get; set; }
        public DateTime Timestamp { get; set; }

    }
}
