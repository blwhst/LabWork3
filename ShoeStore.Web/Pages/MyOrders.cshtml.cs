using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShoeStore.Api.DTOs;
using ShoeStore.Web.Services;

namespace ShoeStore.Web.Pages;

public class MyOrdersModel : PageModel
{
    private readonly ApiClient _api;
    public MyOrdersModel(ApiClient api) => _api = api;

    public List<OrderDto> Orders { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("Login")))
            return RedirectToPage("/Login");

        Orders = await _api.GetMyOrdersAsync() ?? new();
        return Page();
    }
}