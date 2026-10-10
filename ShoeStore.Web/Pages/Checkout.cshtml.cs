using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShoeStore.Api.DTOs;
using ShoeStore.Web.Models;
using ShoeStore.Web.Services;

namespace ShoeStore.Web.Pages;

public class CheckoutModel : PageModel
{
    private readonly ApiClient _api;
    private readonly CartService _cart;

    public CheckoutModel(ApiClient api, CartService cart)
    {
        _api = api;
        _cart = cart;
    }

    public List<CartItem> Items { get; set; } = new();
    public decimal Total { get; set; }

    public List<UserDto> Users { get; set; } = new();

    public string? RoleId { get; set; }

    public async Task OnGetAsync()
    {
        Items = _cart.Get();
        Total = _cart.Total;

        RoleId = HttpContext.Session.GetString("RoleId");

        if (RoleId == RoleIds.Admin || RoleId == RoleIds.Manager)
        {
            var allUsers = await _api.GetUsersAsync() ?? new();
            Users = allUsers.Where(u => u.RoleId.ToString() == RoleIds.User).ToList();
        }
    }

    public async Task<IActionResult> OnPostAsync(int? clientId)
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("Login")))
            return RedirectToPage("/Login");

        var items = _cart.Get();
        if (items.Count == 0) return RedirectToPage("/Cart");

        var roleId = HttpContext.Session.GetString("RoleId");
        var currentUserId = HttpContext.Session.GetInt32("UserId") ?? 0;

        int effectiveUserId;

        if (roleId == RoleIds.Admin || roleId == RoleIds.Manager)
        {
            if (clientId is null || clientId <= 0)
            {
                ModelState.AddModelError(string.Empty, "Выберите пользователя, на которого оформляется заказ.");
                await OnGetAsync();
                return Page();
            }
            effectiveUserId = clientId.Value;
        }
        else
        {
            effectiveUserId = currentUserId;
        }

        var dtoItems = items
            .GroupBy(i => i.ProductItemId)
            .Select(g => new CreateOrderItemDto
            {
                ProductItemId = g.Key,
                Quantity = g.Sum(x => x.Quantity)
            })
            .ToList();

        foreach (var it in dtoItems)
        {
            var cartLine = items.First(i => i.ProductItemId == it.ProductItemId);
            var product = await _api.GetProductAsync(cartLine.ProductId);
            var pi = product?.Items.FirstOrDefault(x => x.ProductItemId == it.ProductItemId);

            if (pi == null)
            {
                ModelState.AddModelError(string.Empty,
                    $"Позиция «{cartLine.ProductName}» (размер {cartLine.Size}) больше недоступна.");
            }
            else if (pi.Quantity < it.Quantity)
            {
                ModelState.AddModelError(string.Empty,
                    $"«{cartLine.ProductName}», размер {cartLine.Size}: " +
                    $"на складе {pi.Quantity} шт., в корзине {it.Quantity} шт.");
            }
        }

        if (!ViewData.ModelState.IsValid)
        {
            await OnGetAsync();
            return Page();
        }

        var dto = new CreateOrderDto
        {
            UserId = effectiveUserId,
            Items = dtoItems
        };

        var result = await _api.CreateOrderAsync(dto);
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty,
                result.Error ?? "Не удалось создать заказ.");
            await OnGetAsync();
            return Page();
        }

        _cart.Clear();
        return RedirectToPage("/MyOrders");
    }
}