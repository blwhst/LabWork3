namespace ShoeStore.Api.DTOs
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int SubcategoryId { get; set; }
        public string SubcategoryName { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string Manufacturer { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public List<MaterialDto> Materials { get; set; } = new();
        public List<ProductItemDto> Items { get; set; } = new();
    }
}
