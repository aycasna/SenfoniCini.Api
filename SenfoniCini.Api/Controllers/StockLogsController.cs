using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SenfoniCini.Api.Data;
using SenfoniCini.Api.DTOs.StockLogs;

namespace SenfoniCini.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class StockLogsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public StockLogsController(ApplicationDbContext context)
        {
            _context = context;
        }

        //GET: api/stocklogs
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var logs = await _context.StockLogs
                .Include(sl => sl.Product)
                .Include(sl => sl.AdminUser)
                .OrderByDescending(sl => sl.Timestamp)
                .ToListAsync();

            var result = logs.Select(sl => new StockLogGetAllDto
            {
                Id = sl.Id,
                ProductId = sl.ProductId,
                ProductName = sl.Product.Name,
                AdminUserId = sl.AdminUserId,
                AdminUserName = sl.AdminUser.UserName,
                Action = sl.Action,
                Remarks = sl.Remarks,
                Timestamp = sl.Timestamp
            });

            return Ok(result);
                
        }
    }
}
