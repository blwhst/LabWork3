namespace ShoeStore.Api.DTOs
{
    public class ProductItemDto
    {
        public int ProductItemId { get; set; }
        public int ProductId { get; set; }
        public decimal Size { get; set; }
        public int Quantity { get; set; }
    }
}
