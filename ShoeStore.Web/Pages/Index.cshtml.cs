using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShoeStore.Api.DTOs;
using ShoeStore.Web.Services;

namespace ShoeStore.Web.Pages;

public class IndexModel : PageModel
{
    private readonly ApiClient _api;
    public IndexModel(ApiClient api) => _api = api;

    public PagedResultDto<ProductDto> Products { get; set; } = new();
    public List<SelectListItem> Categories { get; set; } = new();

    public int? CategoryId { get; set; }
    public string? Sort { get; set; }
    public string? Search { get; set; }

    public async Task OnGetAsync(int? categoryId, string? sort, string? search, int page = 1)
    {
        CategoryId = categoryId;
        Sort = sort;
        Search = search;

        var cats = await _api.GetCategoriesAsync() ?? new();
        Categories = cats.Select(c => new SelectListItem(c.Name, c.CategoryId.ToString())).ToList();

        // Неавторизованный — без фильтров (по ТЗ)
        bool isAuth = !string.IsNullOrEmpty(HttpContext.Session.GetString("Login"));
        if (!isAuth)
        {
            categoryId = null;
            sort = null;
            search = null;
        }

        Products = await _api.GetProductsAsync(categoryId, null, null, search, sort, page, 8)
                   ?? new PagedResultDto<ProductDto>();
    }
}