using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SenfoniCini.Api.Data;
using SenfoniCini.Api.DTOs.Categories;
using SenfoniCini.Api.DTOs.Colors;
using SenfoniCini.Api.DTOs.Products;

namespace SenfoniCini.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        //GET: api/products
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var products = await _context.Products
                .Include(p => p.ProductCategories)
                    .ThenInclude(pca => pca.Category)
                .Include(p => p.ProductColors)
                    .ThenInclude(pco => pco.Color)
                .Select(p => new ProductGetAllDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl,
                    IsSold = p.IsSold,
                    IsDeleted = p.IsDeleted,
                    Categories = p.ProductCategories.Select(pca => new CategoryGetAllDto
                    {
                        Id = pca.Category.Id,
                        Name = pca.Category.Name
                    }).ToList(),
                    Colors = p.ProductColors.Select(pco => new ColorGetAllDto 
                    {
                        Id = pco.Color.Id,
                        Name = pco.Color.Name,
                        HexCode = pco.Color.HexCode
                    }).ToList()
                    

                }).ToListAsync();

            return Ok(products);
        }

        //GET: api/products/{id}
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _context.Products
                .Include(p => p.ProductCategories).ThenInclude(pca => pca.Category)
                .Include(p => p.ProductColors).ThenInclude(pco => pco.Color)
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

            if (product == null)
            {
                return NotFound();
            }

            var dto = new ProductDetailDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                IsSold = product.IsSold,
                IsDeleted = product.IsDeleted,
                Categories = product.ProductCategories.Select(pca => new CategoryGetAllDto
                {
                    Id = pca.Category.Id,
                    Name = pca.Category.Name,
                }).ToList(),
                Colors = product.ProductColors.Select(pco => new ColorGetAllDto
                {
                    Id = pco.Color.Id,
                    Name = pco.Color.Name,
                    HexCode = pco.Color.HexCode
                }).ToList()
            };

            return Ok(dto);
        }

        //POST: /api/products
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] ProductCreateDto dto)
        {
            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                ImageUrl = dto.ImageUrl,
                Price = dto.Price,
                IsSold = false,
                IsDeleted = false,
                
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            if (dto.CategoryIds != null)
            {
                foreach (var categoryId in dto.CategoryIds)
                {
                    _context.ProductCategories.Add(new ProductCategory
                    {
                        ProductId = product.Id,
                        CategoryId = categoryId
                    });

                }
            }
            if (dto.ColorIds != null)
            {
                foreach (var colorId in dto.ColorIds)
                {
                    _context.ProductColors.Add(new ProductColor
                    {
                        ProductId = product.Id,
                        ColorId = colorId
                    });
                }
            }
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, null);
        }

        //PUT: /api/products/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductUpdateDto dto)
        {
            var product = await _context.Products
                .Include(p => p.ProductCategories)
                .Include(p => p.ProductColors)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound();

            product.Name = dto.Name;
            product.Description = dto.Description;
            product.ImageUrl = dto.ImageUrl;
            product.Price = (decimal)dto.Price;

            // Update categories
            product.ProductCategories.Clear();
            if (dto.CategoryIds != null)
            {
                product.ProductCategories = dto.CategoryIds.Select(caid => new ProductCategory
                {
                    CategoryId = caid,
                    ProductId = id
                }).ToList();
            }

            // Update colors
            product.ProductColors.Clear();
            if (dto.ColorIds != null)
            {
                product.ProductColors = dto.ColorIds.Select(coid => new ProductColor
                {
                    ColorId = coid,
                    ProductId = id
                }).ToList();
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // PATCH: /api/products/{id}/is-sold
        [HttpPatch("{id}/is-sold")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateIsSold(int id, [FromBody] ProductUpdateIsSoldDto dto)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            product.IsSold = dto.IsSold;
            await _context.SaveChangesAsync();
            return Ok();
        }


        // PATCH: /api/products/{id}/soft-delete
        [HttpPatch("{id}/soft-delete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SoftDelete(int id, [FromBody] ProductSoftDeleteDto dto)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            product.IsDeleted = dto.IsDeleted;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PATCH: /api/products/bulk-soft-delete
        [HttpPatch("bulk-soft-delete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> BulkSoftDelete([FromBody] ProductBulkSoftDeleteDto dto)
        {
            var products = await _context.Products
                .Where(p => dto.ProductIds.Contains(p.Id))
                .ToListAsync();

            if (products.Count == 0)
                return NotFound("No matching products found.");

            foreach (var product in products)
            {
                product.IsDeleted = dto.IsDeleted;
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

    }
}
