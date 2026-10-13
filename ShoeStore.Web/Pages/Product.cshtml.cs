using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShoeStore.Api.DTOs;
using ShoeStore.Web.Models;
using ShoeStore.Web.Services;

namespace ShoeStore.Web.Pages;

public class ProductModel : PageModel
{
    private readonly ApiClient _api;
    private readonly CartService _cart;

    public ProductModel(ApiClient api, CartService cart)
    {
        _api = api;
        _cart = cart;
    }

    public ProductDto? Product { get; set; }
    public bool IsAuthenticated { get; set; }

    [TempData]
    public string? CartError { get; set; }

    public async Task OnGetAsync(int id)
    {
        Product = await _api.GetProductAsync(id);
        IsAuthenticated = !string.IsNullOrEmpty(HttpContext.Session.GetString("Login"));
    }

    public async Task<IActionResult> OnPostAddAsync(int productId, int productItemId, int quantity)
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("Login")))
            return RedirectToPage("/Login");

        if (quantity <= 0) quantity = 1;

        var product = await _api.GetProductAsync(productId);
        if (product == null)
        {
            CartError = "Товар не найден.";
            return RedirectToPage("/Index");
        }

        var item = product.Items.FirstOrDefault(i => i.ProductItemId == productItemId);
        if (item == null)
        {
            CartError = "Выбранный размер больше не доступен.";
            return RedirectToPage("/Product", new { id = productId });
        }

        var inCart = _cart.Get()
            .FirstOrDefault(c => c.ProductItemId == productItemId)?.Quantity ?? 0;

        if (item.Quantity < inCart + quantity)
        {
            var allowed = Math.Max(0, item.Quantity - inCart);
            CartError = allowed == 0
                ? $"В корзине уже максимум — {inCart} шт. (на складе {item.Quantity})."
                : $"На складе только {item.Quantity} шт. В корзине уже {inCart}, можно добавить не больше {allowed}.";

            return RedirectToPage("/Product", new { id = productId });
        }

        _cart.Add(new CartItem
        {
            ProductItemId = item.ProductItemId,
            ProductId = product.ProductId,
            ProductName = product.Name,
            Image = product.Image,
            Size = item.Size,
            Quantity = quantity,
            Price = product.Price
        });

        return RedirectToPage("/Cart");
    }
}