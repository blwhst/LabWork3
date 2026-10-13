using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShoeStore.Web.Models;
using ShoeStore.Web.Services;

namespace ShoeStore.Web.Pages;

public class LoginModel : PageModel
{
    private readonly ApiClient _api;
    public LoginModel(ApiClient api) => _api = api;

    [BindProperty]
    public LoginViewModel ViewModel { get; set; } = new();

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var result = await _api.LoginAsync(ViewModel.Login);
        if (!result.Success || result.Value == null)
        {
            ViewModel.Error = result.Error ?? "Пользователь с таким логином не найден";
            return Page();
        }

        var user = result.Value;

        HttpContext.Session.SetString("Login", user.Login);
        HttpContext.Session.SetString("RoleId", user.RoleId.ToString());
        HttpContext.Session.SetInt32("UserId", user.UserId);
        HttpContext.Session.SetString("FullName", user.FullName);

        return RedirectToPage("/Index");
    }
}