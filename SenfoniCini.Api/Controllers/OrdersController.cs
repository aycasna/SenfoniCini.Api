using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SenfoniCini.Api.Data;
using SenfoniCini.Api.DTOs.Orders;
using System.Security.Claims;

[Route("api/orders")]
[ApiController]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public OrdersController(ApplicationDbContext context)
    {
        _context = context;
    }

    // POST: api/orders
    [HttpPost]
    public async Task<IActionResult> CreateOrder(OrderCreateDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var cartItems = await _context.CartItems
            .Include(ci => ci.Product)
            .Where(ci => ci.UserId == userId)
            .ToListAsync();

        if (!cartItems.Any())
            return BadRequest("Cart is empty.");

        var order = new Order
        {
            UserId = userId,
            OrderDate = DateTime.UtcNow,
            Status = "Pending",
            ShippingAddress = dto.ShippingAddress,
            ShippingCity = dto.ShippingCity,
            ShippingPostalCode = dto.ShippingPostalCode,
            ShippingFee = dto.ShippingFee,
            TotalAmount = cartItems.Sum(ci => ci.Product.Price) + dto.ShippingFee,
            OrderItems = cartItems.Select(ci => new OrderItem
            {
                ProductId = ci.ProductId,
                Price = ci.Product.Price
            }).ToList()
        };

        _context.Orders.Add(order);
        _context.CartItems.RemoveRange(cartItems);
        await _context.SaveChangesAsync();

        return Ok();
    }

    // GET: api/orders/my
    [HttpGet("my")]
    public async Task<IActionResult> GetCurrentUserOrders()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var orders = await _context.Orders
            .Where(o => o.UserId == userId)
            .Select(o => new OrderCurrentUserGetAllDto
            {
                Id = o.Id,
                TotalAmount = o.TotalAmount,
                Status = o.Status,
                OrderDate = o.OrderDate
            })
            .ToListAsync();

        return Ok(orders);
    }

    // GET: api/orders/my/{id}
    [HttpGet("my/{id}")]
    public async Task<IActionResult> OrderUserDetail(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var order = await _context.Orders
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

        if (order == null)
            return NotFound();

        var dto = new OrderDetailDto
        {
            Id = order.Id,
            TotalAmount = order.TotalAmount,
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
}
