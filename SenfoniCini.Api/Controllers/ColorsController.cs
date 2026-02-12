using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SenfoniCini.Api.Data;
using SenfoniCini.Api.DTOs.Colors;

namespace SenfoniCini.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ColorsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ColorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/colors
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var colors = await _context.Colors
                .Select(c => new ColorGetAllDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    HexCode = c.HexCode,
                }).ToListAsync();

            return Ok(colors);
        }

        // GET: api/colors/{id}
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var color = await _context.Colors
                .Where(c => c.Id == id)
                .Select(c => new ColorGetAllDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    HexCode = c.HexCode,
                }).FirstOrDefaultAsync();
            if (color == null)
            {
                return NotFound();
            }
            return Ok(color);
        }

        // POST: api/colors
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] ColorCreateDto colorDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var color = new Color
            {
                Name = colorDto.Name,
                HexCode = colorDto.HexCode,
            };
            _context.Colors.Add(color);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAll), new { id = color.Id }, color);
        }

        // PUT: api/colors/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] ColorUpdateDto colorDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var color = await _context.Colors.FindAsync(id);
            if (color == null)
            {
                return NotFound();
            }
            color.Name = colorDto.Name;
            color.HexCode = colorDto.HexCode;
            _context.Colors.Update(color);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/colors/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var color = await _context.Colors.FindAsync(id);
            if (color == null)
            {
                return NotFound();
            }
            _context.Colors.Remove(color);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/colors/bulk-delete
        [HttpDelete("bulk-delete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> BulkDelete([FromBody] List<int> ids)
        {
            if (ids == null || !ids.Any())
            {
                return BadRequest("No color IDs provided for deletion.");
            }
            var colors = await _context.Colors.Where(c => ids.Contains(c.Id)).ToListAsync();
            if (!colors.Any())
            {
                return NotFound("No colors found for the provided IDs.");
            }
            _context.Colors.RemoveRange(colors);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}

