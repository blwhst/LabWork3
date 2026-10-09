using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShoeStore.Api.DTOs;
using ShoeStore.Web.Services;

namespace ShoeStore.Web.Pages.Admin;

public class EditOrderModel : PageModel
{
    private readonly ApiClient _api;
    public EditOrderModel(ApiClient api) => _api = api;

    public int? OrderId { get; set; }
    public int UserId { get; set; }
    public List<UserDto> Users { get; set; } = new();
    public List<OrderItemDto> Items { get; set; } = new();

    [TempData]
    public string? Error { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (HttpContext.Session.GetString("RoleId") != "1")
            return RedirectToPage("/Login");

        Users = await _api.GetUsersAsync() ?? new();

        if (id.HasValue)
        {
            var order = await _api.GetOrderAsync(id.Value);
            if (order != null)
            {
                OrderId = order.OrderId;
                UserId = order.UserId;
                Items = order.Items;
            }
        }
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int userId)
    {
        if (HttpContext.Session.GetString("RoleId") != "1")
            return RedirectToPage("/Login");

        if (!OrderId.HasValue)
        {
            Error = "Создание нового заказа через эту форму не поддерживается.";
            return RedirectToPage("/Admin/Orders");
        }

        // Пересобираем состав заказа из уже загруженных позиций
        var dto = new UpdateOrderDto
        {
            UserId = userId,
            Items = Items
                .GroupBy(i => i.ProductItemId)
                .Select(g => new CreateOrderItemDto
                {
                    ProductItemId = g.Key,
                    Quantity = g.Sum(x => x.Quantity)
                })
                .ToList()
        };

        var res = await _api.UpdateOrderAsync(OrderId.Value, dto);
        if (!res.Success)
        {
            Error = res.Error ?? "Не удалось сохранить заказ.";
            return RedirectToPage(new { id = OrderId.Value });
        }

        return RedirectToPage("/Admin/Orders");
    }
}