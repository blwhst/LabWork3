namespace ShoeStore.Api.DTOs
{
    public class CreateOrderDto
    {
        public int UserId { get; set; }
        public List<CreateOrderItemDto> Items { get; set; } = new();
    }
}
