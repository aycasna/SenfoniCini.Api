using System.ComponentModel.DataAnnotations;

namespace SenfoniCini.Api.DTOs.Colors
{
    public class ColorCreateDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string HexCode { get; set; }
        }
}
