namespace ShoeStore.Api.DTOs
{
    public class UpdateOrderDto
    {
        public int UserId { get; set; }
        public List<CreateOrderItemDto> Items { get; set; } = new();
    }
}
