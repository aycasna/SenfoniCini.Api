namespace SenfoniCini.Api.DTOs.Products
{
    public class ProductUpdateDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public decimal? Price { get; set; }
        public bool? IsSold { get; set; }
        public bool? IsDeleted { get; set; }
        public List<int>? CategoryIds { get; set; }
        public List<int>? ColorIds { get; set; }
    }
}
