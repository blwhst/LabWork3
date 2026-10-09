using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShoeStore.Api.DTOs;
using ShoeStore.Web.Services;

namespace ShoeStore.Web.Pages;

public class OrderDetailsModel : PageModel
{
    private readonly ApiClient _api;
    public OrderDetailsModel(ApiClient api) => _api = api;

    public OrderDto? Order { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("Login")))
            return RedirectToPage("/Login");

        var order = await _api.GetOrderAsync(id);
        if (order == null) return NotFound();

        var roleId = HttpContext.Session.GetString("RoleId");
        var userId = HttpContext.Session.GetInt32("UserId");

        if (roleId == "3" && order.UserId != userId)
            return Forbid();

        Order = order;
        return Page();
    }
}