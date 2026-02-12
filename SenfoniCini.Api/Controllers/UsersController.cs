using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SenfoniCini.Api.Data;
using SenfoniCini.Api.DTOs.Orders;
using SenfoniCini.Api.DTOs.Users;

namespace SenfoniCini.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        public UsersController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        //GET: api/users
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userManager.GetUsersInRoleAsync("User");
            var result = users
                .Where(u => !u.IsDeleted)
                .Select(u => new UserGetAllDto
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    IsDeleted = u.IsDeleted
                }).ToList();

            return Ok(result);
        }

        //GET: api/users/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null || user.IsDeleted)
                return NotFound();

            var isUser = await _userManager.IsInRoleAsync(user, "User");
            if (!isUser)
                return BadRequest("Not a user.");

            var orders = await _context.Orders
                .Where(o => o.UserId == user.Id)
                .Include(o => o.OrderItems)
                .Select(o => new OrderGetAllDto
                {
                    Id = o.Id,
                    UserId = o.UserId,
                    TotalAmount = o.TotalAmount,
                    ShippingFee = o.ShippingFee,
                    Status = o.Status,
                    OrderDate = o.OrderDate
                }).ToListAsync();



            var dto = new UserDetailDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                City = user.City,
                ZipCode = user.ZipCode,
                IsDeleted = user.IsDeleted,
                Orders = orders
            };

            return Ok(dto);
        }

        [HttpPatch("{id}/soft-delete")]
        public async Task<IActionResult> SoftDelete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null || user.IsDeleted)
                return NotFound();

            var isUser = await _userManager.IsInRoleAsync(user, "User");
            if (!isUser)
                return BadRequest("Not a user.");

            user.IsDeleted = true;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return StatusCode(500, "Error soft deleting user.");
            return NoContent();
        }

        [HttpPatch("bulk-soft-delete")]
        public async Task<IActionResult> BulkSoftDelete([FromBody] UserBulkSoftDeleteDto dto)
        {
            
            foreach (var id in dto.Ids)
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user != null && !user.IsDeleted && await _userManager.IsInRoleAsync(user, "User"))
                {
                    user.IsDeleted = true;
                    await _userManager.UpdateAsync(user);
                }
            }

            return NoContent();
        }
    }
}
