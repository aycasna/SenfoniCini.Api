using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SenfoniCini.Api.Data;
using SenfoniCini.Api.DTOs.Orders;
using System.Numerics;


namespace SenfoniCini.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        //GET: api/orders
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _context.Orders
                .Select(o => new OrderGetAllDto
                {
                    Id = o.Id,
                    UserId = o.UserId,
                    UserFirstName = o.User.FirstName,
                    UserLastName = o.User.LastName,
                    UserEmail = o.User.Email,
                    TotalAmount = o.TotalAmount,
                    ShippingFee = o.ShippingFee,
                    Status = o.Status,
                    OrderDate = o.OrderDate,

                }).ToListAsync();

            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if(order == null)
            {
                return NotFound();
            }

            var dto = new OrderDetailDto
            {
                Id = order.Id,
                UserId = order.User.Id,
                UserFirstName = order.User.FirstName,
                UserLastName = order.User.LastName,
                UserEmail = order.User.Email,
                TotalAmount = order.TotalAmount,
                ShippingFee = order.ShippingFee,
                Status = order.Status,
                OrderDate = order.OrderDate,
                ShippingAddress = order.ShippingAddress,
                ShippingCity = order.ShippingCity,
                ShippingPostalCode = order.ShippingPostalCode,
                OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product.Name,
                    ProductPrice = oi.Price
                }).ToList()
            };

            return Ok(dto);
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] OrderUpdateStatusDto dto)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            order.Status = dto.Status;
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
