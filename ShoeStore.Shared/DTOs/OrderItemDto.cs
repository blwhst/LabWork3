namespace ShoeStore.Api.DTOs
{
    public class OrderItemDto
    {
        public int OrderItemId { get; set; }
        public int ProductItemId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Size { get; set; }
        public int Quantity { get; set; }
        public decimal PricePerUnit { get; set; }
        public decimal Total => PricePerUnit * Quantity;
    }
}
