using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SenfoniCini.Api.Data;
using SenfoniCini.Api.DTOs.Categories;

namespace SenfoniCini.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/categories
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _context.Categories
                .Select(c => new CategoryGetAllDto
                {
                    Id = c.Id,
                    Name = c.Name
                }).ToListAsync();

            return Ok(categories);
        }

        // GET: api/categories/{id}
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _context.Categories
                .Where(c => c.Id == id)
                .Select(c => new CategoryGetAllDto
                {
                    Id = c.Id,
                    Name = c.Name
                }).FirstOrDefaultAsync();
            if (category == null)
            {
                return NotFound();
            }
            return Ok(category);
        }

        // POST: api/categories
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CategoryCreateDto categoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var category = new Category
            {
                Name = categoryDto.Name
            };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAll), new { id = category.Id }, category);
        }

        // PUT: api/categories/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] CategoryUpdateDto categoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            category.Name = categoryDto.Name;
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/categories/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/categories/bulk-delete
        [HttpDelete("bulk-delete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> BulkDelete([FromBody] List<int> ids)
        {
            if (ids == null || !ids.Any())
            {
                return BadRequest("No category IDs provided for deletion.");
            }
            var categories = await _context.Categories.Where(c => ids.Contains(c.Id)).ToListAsync();
            if (!categories.Any())
            {
                return NotFound("No categories found for the provided IDs.");
            }
            _context.Categories.RemoveRange(categories);
            await _context.SaveChangesAsync();
            return NoContent();
        }

    }


}
