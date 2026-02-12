using SenfoniCini.Api.DTOs.Categories;
using System.ComponentModel.DataAnnotations;

namespace SenfoniCini.Api.DTOs.Products
{
    public class ProductCreateDto
    {
        [Required]
        public string Name { get; set; }

        //Optional
        public string? Description { get; set; }
        
        //Optional
        public string? ImageUrl { get; set; }

        [Required]
        public decimal Price { get; set; }

        //Optional
        public List<int> CategoryIds { get; set; }


        //Optional
        public List<int> ColorIds { get; set; }



    }
}
