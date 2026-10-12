using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShoeStore.Api.DTOs;
using ShoeStore.Web.Models;
using ShoeStore.Web.Services;

namespace ShoeStore.Web.Pages;

public class OrdersModel : PageModel
{
    private readonly ApiClient _api;
    public OrdersModel(ApiClient api) => _api = api;

    public List<OrderDto> Orders { get; set; } = new();
    public bool IsAdmin { get; set; }

    // Хранит текущий выбранный тип сортировки
    public string CurrentSort { get; set; } = "desc";

    [TempData]
    public string? Error { get; set; }

    public async Task<IActionResult> OnGetAsync(string sortByDate = "desc")
    {
        var roleId = HttpContext.Session.GetString("RoleId");
        if (roleId != RoleIds.Admin && roleId != RoleIds.Manager)
            return RedirectToPage("/Login");

        IsAdmin = roleId == RoleIds.Admin;
        CurrentSort = sortByDate;

        var rawOrders = await _api.GetOrdersAsync() ?? new();

        // Логика сортировки списка заказов
        Orders = sortByDate == "asc"
            ? rawOrders.OrderBy(o => o.OrderDate).ToList()
            : rawOrders.OrderByDescending(o => o.OrderDate).ToList();

        return Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var roleId = HttpContext.Session.GetString("RoleId");
        if (roleId != RoleIds.Admin && roleId != RoleIds.Manager)
            return RedirectToPage("/Login");

        var res = await _api.DeleteOrderAsync(id);
        if (!res.Success)
            Error = res.Error ?? "Не удалось удалить заказ.";

        return RedirectToPage();
    }
}