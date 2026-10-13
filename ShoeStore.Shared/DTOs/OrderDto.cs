namespace ShoeStore.Api.DTOs
{
    public class OrderDto
    {
        public int OrderId { get; set; }
        public DateOnly OrderDate { get; set; }
        public int UserId { get; set; }
        public string CustomerFullName { get; set; } = string.Empty;
        public List<OrderItemDto> Items { get; set; } = new();
        public decimal TotalSum => Items.Sum(i => i.Total);
    }
}
