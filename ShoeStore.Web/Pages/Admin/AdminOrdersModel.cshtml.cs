using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShoeStore.Api.DTOs;
using ShoeStore.Web.Services;

namespace ShoeStore.Web.Pages.Admin;

public class AdminOrdersModel : PageModel
{
    private readonly ApiClient _api;
    public AdminOrdersModel(ApiClient api) => _api = api;

    public List<OrderDto> Orders { get; set; } = new();

    [TempData]
    public string? Error { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        if (HttpContext.Session.GetString("RoleId") != "1")
            return RedirectToPage("/Login");

        Orders = await _api.GetOrdersAsync() ?? new();
        return Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        // Проверка роли должна быть и в POST-обработчике
        if (HttpContext.Session.GetString("RoleId") != "1")
            return RedirectToPage("/Login");

        var res = await _api.DeleteOrderAsync(id);
        if (!res.Success)
            Error = res.Error ?? "Не удалось удалить заказ.";

        return RedirectToPage();
    }
}