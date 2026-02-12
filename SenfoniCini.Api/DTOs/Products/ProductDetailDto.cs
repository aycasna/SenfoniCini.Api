using SenfoniCini.Api.DTOs.Categories;
using SenfoniCini.Api.DTOs.Colors;

namespace SenfoniCini.Api.DTOs.Products
{
    public class ProductDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
       
        public string ImageUrl { get; set; }
   
        public bool IsSold { get; set; }
        public bool IsDeleted { get; set; }
        public List<CategoryGetAllDto> Categories { get; set; }
        public List<ColorGetAllDto> Colors { get; set; }
    }
}
