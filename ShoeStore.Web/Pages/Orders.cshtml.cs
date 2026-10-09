using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShoeStore.Api.DTOs;
using ShoeStore.Web.Services;

namespace ShoeStore.Web.Pages;

public class OrdersModel : PageModel
{
    private readonly ApiClient _api;
    public OrdersModel(ApiClient api) => _api = api;

    public List<OrderDto> Orders { get; set; } = new();
    public bool IsAdmin { get; set; }

    [TempData]
    public string? Error { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var roleId = HttpContext.Session.GetString("RoleId");
        if (roleId != "1" && roleId != "2")
            return RedirectToPage("/Login");

        IsAdmin = roleId == "1";
        Orders = await _api.GetOrdersAsync() ?? new();
        return Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        // Повторная проверка в POST-обработчике
        var roleId = HttpContext.Session.GetString("RoleId");
        if (roleId != "1" && roleId != "2")
            return RedirectToPage("/Login");

        var res = await _api.DeleteOrderAsync(id);
        if (!res.Success)
            Error = res.Error ?? "Не удалось удалить заказ.";

        return RedirectToPage();
    }
}