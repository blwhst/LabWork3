using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShoeStore.Web.Services;

namespace ShoeStore.Web.Pages;

public class LogoutModel : PageModel
{
    private readonly ApiClient _api;
    public LogoutModel(ApiClient api) => _api = api;

    public async Task<IActionResult> OnGetAsync()
    {
        await _api.LogoutAsync();
        HttpContext.Session.Clear();
        return RedirectToPage("/Index");
    }
}