using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShoeStore.Web.Models;
using ShoeStore.Web.Services;

namespace ShoeStore.Web.Pages;

public class CartModel : PageModel
{
    private readonly CartService _cart;
    private readonly ApiClient _api;

    public CartModel(CartService cart, ApiClient api)
    {
        _cart = cart;
        _api = api;
    }

    public List<CartItem> Items { get; set; } = new();
    /// <summary>Доступный остаток по ProductItemId.</summary>
    public Dictionary<int, int> Stock { get; set; } = new();
    public decimal Total { get; set; }

    [TempData]
    public string? Error { get; set; }

    public async Task OnGetAsync()
    {
        Items = _cart.Get();
        Total = _cart.Total;

        // Актуальные остатки на каждую позицию корзины
        foreach (var it in Items)
        {
            var product = await _api.GetProductAsync(it.ProductId);
            var pi = product?.Items.FirstOrDefault(x => x.ProductItemId == it.ProductItemId);
            Stock[it.ProductItemId] = pi?.Quantity ?? 0;
        }
    }

    public async Task<IActionResult> OnPostUpdateAsync(int productItemId, int qty)
    {
        if (qty <= 0)
        {
            _cart.Remove(productItemId);
            return RedirectToPage();
        }

        var cartItem = _cart.Get().FirstOrDefault(c => c.ProductItemId == productItemId);
        if (cartItem == null)
        {
            _cart.Remove(productItemId);
            return RedirectToPage();
        }

        // Свежий остаток
        var product = await _api.GetProductAsync(cartItem.ProductId);
        var item = product?.Items.FirstOrDefault(i => i.ProductItemId == productItemId);

        if (item == null)
        {
            Error = "Товарная позиция больше не существует.";
            _cart.Remove(productItemId);
            return RedirectToPage();
        }

        if (qty > item.Quantity)
        {
            Error = $"«{cartItem.ProductName}» (размер {cartItem.Size}): " +
                    $"на складе только {item.Quantity} шт. Значение скорректировано.";
            qty = item.Quantity;
        }

        _cart.UpdateQuantity(productItemId, qty);
        return RedirectToPage();
    }

    public IActionResult OnPostRemove(int productItemId)
    {
        _cart.Remove(productItemId);
        return RedirectToPage();
    }
}