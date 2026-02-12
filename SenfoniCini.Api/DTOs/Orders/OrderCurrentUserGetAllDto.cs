namespace SenfoniCini.Api.DTOs.Orders
{
    public class OrderCurrentUserGetAllDto
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
    }
}
