namespace ShoeStore.Web.Models;

public class CartItem
{
    public int ProductItemId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public decimal Size { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal Sum => Price * Quantity;
}