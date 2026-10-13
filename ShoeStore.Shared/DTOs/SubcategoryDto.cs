namespace ShoeStore.Api.DTOs
{
    public class SubcategoryDto
    {
        public int SubcategoryId { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
    }
}
