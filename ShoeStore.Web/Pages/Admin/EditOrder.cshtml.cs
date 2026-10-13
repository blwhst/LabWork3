using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShoeStore.Api.DTOs;
using ShoeStore.Web.Models;
using ShoeStore.Web.Services;

namespace ShoeStore.Web.Pages.Admin;

public class EditOrderModel : PageModel
{
    private readonly ApiClient _api;
    public EditOrderModel(ApiClient api) => _api = api;

    [BindProperty]
    public int? OrderId { get; set; }

    public int UserId { get; set; }
    public List<UserDto> Users { get; set; } = new();

    [BindProperty]
    public List<OrderItemDto> Items { get; set; } = new();

    public Dictionary<int, int> Stock { get; set; } = new();

    [TempData]
    public string? Error { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (HttpContext.Session.GetString("RoleId") != RoleIds.Admin)
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

                foreach (var it in Items)
                {
                    var product = await _api.GetProductAsync(it.ProductId);
                    var pi = product?.Items.FirstOrDefault(x => x.ProductItemId == it.ProductItemId);
                    Stock[it.ProductItemId] = pi?.Quantity ?? 0;
                }
            }
        }
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int userId, int? removeIndex)
    {
        if (HttpContext.Session.GetString("RoleId") != RoleIds.Admin)
            return RedirectToPage("/Login");

        if (!OrderId.HasValue)
        {
            Error = "Создание нового заказа через эту форму не поддерживается.";
            return RedirectToPage("/Orders");
        }

        if (removeIndex.HasValue && removeIndex.Value >= 0 && removeIndex.Value < Items.Count)
        {
            Items.RemoveAt(removeIndex.Value);

            var dtoRemove = new UpdateOrderDto
            {
                UserId = userId,
                Items = Items
                    .Where(i => i.Quantity > 0)
                    .GroupBy(i => i.ProductItemId)
                    .Select(g => new CreateOrderItemDto
                    {
                        ProductItemId = g.Key,
                        Quantity = g.Sum(x => x.Quantity)
                    })
                    .ToList()
            };

            if (dtoRemove.Items.Count == 0)
            {
                Error = "Заказ не может быть пустым. Удалите заказ целиком или оставьте хотя бы одну позицию.";
                return RedirectToPage(new { id = OrderId.Value });
            }

            var resRemove = await _api.UpdateOrderAsync(OrderId.Value, dtoRemove);
            if (!resRemove.Success)
            {
                Error = resRemove.Error ?? "Не удалось удалить позицию.";
            }

            return RedirectToPage(new { id = OrderId.Value });
        }

        var dto = new UpdateOrderDto
        {
            UserId = userId,
            Items = Items
                .Where(i => i.Quantity > 0)
                .GroupBy(i => i.ProductItemId)
                .Select(g => new CreateOrderItemDto
                {
                    ProductItemId = g.Key,
                    Quantity = g.Sum(x => x.Quantity)
                })
                .ToList()
        };

        if (dto.Items.Count == 0)
        {
            Error = "Заказ не может быть пустым.";
            return RedirectToPage(new { id = OrderId.Value });
        }

        var res = await _api.UpdateOrderAsync(OrderId.Value, dto);
        if (!res.Success)
        {
            Error = res.Error ?? "Не удалось сохранить заказ.";
            return RedirectToPage(new { id = OrderId.Value });
        }

        return RedirectToPage("/Orders");
    }
}