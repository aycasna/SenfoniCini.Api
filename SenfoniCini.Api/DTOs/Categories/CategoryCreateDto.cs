using System.ComponentModel.DataAnnotations;

namespace SenfoniCini.Api.DTOs.Categories
{
    public class CategoryCreateDto
    {
        [Required]
        public string Name { get; set; }
    }
}
