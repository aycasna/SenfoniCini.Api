using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SenfoniCini.Api.Data;
using SenfoniCini.Api.DTOs.CartItems;

namespace SenfoniCini.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartItemsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartItemsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /api/cartitems
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = _userManager.GetUserId(User);

            var cartItems = await _context.CartItems
                .Where(ci => ci.UserId == userId)
                .Include(ci => ci.Product)
                .Select(ci => new CartItemGetAllDto
                {
                    Id = ci.Id,
                    ProductId = ci.ProductId,
                    ProductName = ci.Product.Name,
                    ProductImageUrl = ci.Product.ImageUrl,
                    ProductPrice = ci.Product.Price
                })
                .ToListAsync();

            return Ok(cartItems);
        }

        // POST: /api/cartitems
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CartItemCreateDto dto)
        {
            var userId = _userManager.GetUserId(User);

            var product = await _context.Products
                .FirstOrDefaultAsync(p =>
                    p.Id == dto.ProductId &&
                    !p.IsDeleted &&
                    !p.IsSold);

            if (product == null)
                return BadRequest("Product is not available.");

            var alreadyInCart = await _context.CartItems
                .AnyAsync(ci =>
                    ci.UserId == userId &&
                    ci.ProductId == dto.ProductId);

            if (alreadyInCart)
                return BadRequest("Product already in cart.");

            var cartItem = new CartItem
            {
                UserId = userId,
                ProductId = dto.ProductId
            };

            _context.CartItems.Add(cartItem);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // DELETE: /api/cartitems/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userManager.GetUserId(User);

            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci =>
                    ci.Id == id &&
                    ci.UserId == userId);

            if (cartItem == null)
                return NotFound();

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
