using SenfoniCini.Api.DTOs.StockLogs;

namespace SenfoniCini.Api.DTOs.Admins
{
    public class AdminDetailDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public bool IsDeleted { get; set; }
        public List<StockLogGetAllDto> StockLogs { get; set; }
    }
}
