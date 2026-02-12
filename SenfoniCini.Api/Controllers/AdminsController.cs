using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SenfoniCini.Api.Data;
using SenfoniCini.Api.DTOs.Admins;
using SenfoniCini.Api.DTOs.StockLogs;
using SenfoniCini.Api.DTOs.Users;

namespace SenfoniCini.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminsController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public AdminsController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        //GET: api/admins
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var admins = await _userManager.GetUsersInRoleAsync("Admin");
            var result = admins
                .Where(a => !a.IsDeleted)
                .Select(a => new AdminGetAllDto
                {
                    Id = a.Id,
                    Email = a.Email,
                    IsDeleted = a.IsDeleted
                }).ToList();

            return Ok(result);
        }

        //GET: api/admins/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var admin = await _userManager.FindByIdAsync(id);
            if (admin == null || admin.IsDeleted)
                return NotFound();

            var isAdmin = await _userManager.IsInRoleAsync(admin, "Admin");
            if (!isAdmin)
                return BadRequest("User is not an admin.");

            var logs = await _context.StockLogs
                .Where(l => l.AdminUserId == admin.Id)
                .Include(l => l.Product)
                .OrderByDescending(l => l.Timestamp)
                .Select(l => new StockLogGetAllDto
                {
                    Id = l.Id,
                    ProductId = l.ProductId,
                    ProductName = l.Product.Name,
                    Action = l.Action,
                    Remarks = l.Remarks,
                    Timestamp = l.Timestamp
                }).ToListAsync();

            var dto = new AdminDetailDto
            {
                Id = admin.Id,
                Email = admin.Email,
                IsDeleted = admin.IsDeleted,
                StockLogs = logs
            };

            return Ok(dto);
        }

        //POST: /api/admins
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AdminCreateDto dto)
        {
            var admin = new ApplicationUser
            {
                Email = dto.Email,
                UserName = dto.Email,
                FirstName = dto.FirstName ?? "Admin",
                LastName = dto.LastName ?? "User",
                PhoneNumber = dto.PhoneNumber ?? "111",
                Address = dto.Address ?? "N/A",
                City = dto.City ?? "N/A",
                ZipCode = dto.ZipCode ?? "00000",
                EmailConfirmed = true,
                IsDeleted = false
            };

            var result = await _userManager.CreateAsync(admin, dto.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            await _userManager.AddToRoleAsync(admin, "Admin");

            return Ok(new { admin.Id, admin.Email });
        }


        //PUT: /api/admins/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] AdminUpdateDto dto)
        {
            var admin = await _userManager.FindByIdAsync(id);
            if (admin == null || admin.IsDeleted)
                return NotFound();

            admin.Email = dto.Email;
            admin.UserName = dto.Email;

            var updateResult = await _userManager.UpdateAsync(admin);
            if (!updateResult.Succeeded)
                return BadRequest(updateResult.Errors);

            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(admin);
                var passwordResult = await _userManager.ResetPasswordAsync(admin, token, dto.Password);
                if (!passwordResult.Succeeded)
                    return BadRequest(passwordResult.Errors);
            }

            return NoContent();
        }

        [HttpPatch("{id}/soft-delete")]
        public async Task<IActionResult> SoftDelete(string id)
        {
            var admin = await _userManager.FindByIdAsync(id);
            if (admin == null || admin.IsDeleted)
                return NotFound();

            var isAdmin = await _userManager.IsInRoleAsync(admin, "Admin");
            if (!isAdmin)
                return BadRequest("User is not an admin.");

            admin.IsDeleted = true;
            var result = await _userManager.UpdateAsync(admin);
            if(!result.Succeeded)
                return StatusCode(500, "Error soft deleting admin.");
            return NoContent();
        }

        [HttpPatch("bulk-soft-delete")]
        public async Task<IActionResult> BulkSoftDelete([FromBody] UserBulkSoftDeleteDto dto)
        {
            

            foreach (var id in dto.Ids)
            {
                var admin = await _userManager.FindByIdAsync(id);
                if (admin != null && !admin.IsDeleted && await _userManager.IsInRoleAsync(admin, "Admin"))
                {
                    admin.IsDeleted = true;
                    await _userManager.UpdateAsync(admin);
                }
            }

            return NoContent();

        }
    }
}
