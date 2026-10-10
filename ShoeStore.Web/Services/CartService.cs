using System.Text.Json;
using ShoeStore.Web.Models;

namespace ShoeStore.Web.Services;

public class CartService
{
    private const string Key = "cart";
    private readonly ISession _session;

    public CartService(IHttpContextAccessor ctx)
    {
        _session = ctx.HttpContext!.Session;
    }

    public List<CartItem> Get()
    {
        var json = _session.GetString(Key);
        return json == null
            ? new List<CartItem>()
            : JsonSerializer.Deserialize<List<CartItem>>(json)!;
    }

    public void Save(List<CartItem> items)
        => _session.SetString(Key, JsonSerializer.Serialize(items));

    public void Add(CartItem item)
    {
        var items = Get();
        var existing = items.FirstOrDefault(i => i.ProductItemId == item.ProductItemId);
        if (existing != null) existing.Quantity += item.Quantity;
        else items.Add(item);
        Save(items);
    }

    public void UpdateQuantity(int productItemId, int quantity)
    {
        var items = Get();
        var it = items.FirstOrDefault(i => i.ProductItemId == productItemId);
        if (it == null) return;

        if (quantity <= 0) items.Remove(it);
        else it.Quantity = quantity;

        Save(items);
    }

    public void Remove(int productItemId)
    {
        var items = Get();
        items.RemoveAll(i => i.ProductItemId == productItemId);
        Save(items);
    }

    public void Clear() => _session.Remove(Key);

    public decimal Total => Get().Sum(i => i.Price * i.Quantity);
}