namespace SenfoniCini.Api.DTOs.Products
{
    public class ProductBulkSoftDeleteDto
    {
        public List<int> ProductIds { get; set; } 
        public bool IsDeleted { get; set; }
    }
}
