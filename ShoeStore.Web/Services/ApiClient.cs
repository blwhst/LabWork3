using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ShoeStore.Api.DTOs;

namespace ShoeStore.Web.Services;

public class ApiClient
{
    private readonly HttpClient _http;
    private readonly IHttpContextAccessor _ctx;
    private readonly ILogger<ApiClient> _logger;

    public ApiClient(HttpClient http, IHttpContextAccessor ctx, ILogger<ApiClient> logger)
    {
        _http = http;
        _ctx = ctx;
        _logger = logger;
    }

    // ------------------ helpers ------------------

    private HttpRequestMessage Build(HttpMethod method, string url, HttpContent? content = null)
    {
        var req = new HttpRequestMessage(method, url) { Content = content };
        var s = _ctx.HttpContext?.Session;
        if (s != null)
        {
            var uid = s.GetInt32("UserId");
            var rid = s.GetString("RoleId");
            var lg = s.GetString("Login");
            if (uid.HasValue) req.Headers.Add("X-User-Id", uid.Value.ToString());
            if (rid != null) req.Headers.Add("X-Role-Id", rid);
            if (lg != null) req.Headers.Add("X-Login", lg);
        }
        return req;
    }

    private async Task<ApiResult<T>> SendAsync<T>(HttpRequestMessage req)
    {
        try
        {
            var resp = await _http.SendAsync(req);

            if (resp.IsSuccessStatusCode)
            {
                if (resp.StatusCode == HttpStatusCode.NoContent)
                    return ApiResult<T>.Ok(default!);

                var value = await resp.Content.ReadFromJsonAsync<T>();
                return ApiResult<T>.Ok(value!);
            }

            var body = await resp.Content.ReadAsStringAsync();
            var msg = ExtractError(body) ?? resp.StatusCode switch
            {
                HttpStatusCode.Unauthorized => "Требуется вход в систему.",
                HttpStatusCode.Forbidden => "Недостаточно прав.",
                HttpStatusCode.NotFound => "Запись не найдена.",
                HttpStatusCode.Conflict => "Конфликт данных. Проверьте остатки.",
                _ => "Сервис временно недоступен."
            };

            return ApiResult<T>.Fail((int)resp.StatusCode, msg);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error calling {Url}", req.RequestUri);
            return ApiResult<T>.Fail(503, "Сервис временно недоступен.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error calling {Url}", req.RequestUri);
            return ApiResult<T>.Fail(500, "Внутренняя ошибка.");
        }
    }

    private static string? ExtractError(string body)
    {
        if (string.IsNullOrWhiteSpace(body)) return null;
        try
        {
            using var doc = JsonDocument.Parse(body);
            if (doc.RootElement.TryGetProperty("error", out var e))
                return e.GetString();
        }
        catch { /* not json */ }
        return null;
    }

    // ------------------ AUTH ------------------

    public async Task<LoginResponseDto?> LoginAsync(string login)
    {
        var req = new HttpRequestMessage(HttpMethod.Post, "api/auth/login")
        {
            Content = JsonContent.Create(new LoginDto { Login = login })
        };
        var resp = await _http.SendAsync(req);
        if (!resp.IsSuccessStatusCode) return null;
        return await resp.Content.ReadFromJsonAsync<LoginResponseDto>();
    }

    public async Task LogoutAsync()
    {
        var req = Build(HttpMethod.Post, "api/auth/logout");
        await _http.SendAsync(req);
    }

    // ------------------ CATALOG ------------------

    public async Task<PagedResultDto<ProductDto>?> GetProductsAsync(
        int? categoryId = null, int? subcategoryId = null,
        string? manufacturer = null, string? search = null,
        string? sort = null, int page = 1, int size = 10)
    {
        var url = $"api/products?page={page}&size={size}";
        if (categoryId.HasValue) url += $"&categoryId={categoryId}";
        if (subcategoryId.HasValue) url += $"&subcategoryId={subcategoryId}";
        if (!string.IsNullOrEmpty(manufacturer)) url += $"&manufacturer={Uri.EscapeDataString(manufacturer)}";
        if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
        if (!string.IsNullOrEmpty(sort)) url += $"&sort={Uri.EscapeDataString(sort)}";

        var req = Build(HttpMethod.Get, url);
        var res = await SendAsync<PagedResultDto<ProductDto>>(req);
        return res.Value;
    }

    public async Task<ProductDto?> GetProductAsync(int id)
    {
        var req = Build(HttpMethod.Get, $"api/products/{id}");
        var res = await SendAsync<ProductDto>(req);
        return res.Value;
    }

    public async Task<List<ProductItemDto>?> GetProductSizesAsync(int id)
    {
        var req = Build(HttpMethod.Get, $"api/products/{id}/sizes");
        var res = await SendAsync<List<ProductItemDto>>(req);
        return res.Value;
    }

    public async Task<List<CategoryDto>?> GetCategoriesAsync()
    {
        var req = Build(HttpMethod.Get, "api/categories");
        var res = await SendAsync<List<CategoryDto>>(req);
        return res.Value;
    }

    public async Task<List<SubcategoryDto>?> GetSubcategoriesAsync(int categoryId)
    {
        var req = Build(HttpMethod.Get, $"api/categories/{categoryId}/subcategories");
        var res = await SendAsync<List<SubcategoryDto>>(req);
        return res.Value;
    }

    public async Task<List<MaterialDto>?> GetMaterialsAsync()
    {
        var req = Build(HttpMethod.Get, "api/categories/materials");
        var res = await SendAsync<List<MaterialDto>>(req);
        return res.Value;
    }

    // ------------------ ORDERS ------------------

    public async Task<List<OrderDto>?> GetOrdersAsync()
    {
        var req = Build(HttpMethod.Get, "api/orders");
        var res = await SendAsync<List<OrderDto>>(req);
        return res.Value;
    }

    public async Task<List<OrderDto>?> GetMyOrdersAsync()
    {
        var req = Build(HttpMethod.Get, "api/orders/my");
        var res = await SendAsync<List<OrderDto>>(req);
        return res.Value;
    }

    public async Task<OrderDto?> GetOrderAsync(int id)
    {
        var req = Build(HttpMethod.Get, $"api/orders/{id}");
        var res = await SendAsync<OrderDto>(req);
        return res.Value;
    }

    public async Task<ApiResult<OrderDto>> CreateOrderAsync(CreateOrderDto dto)
    {
        var req = Build(HttpMethod.Post, "api/orders", JsonContent.Create(dto));
        return await SendAsync<OrderDto>(req);
    }

    public async Task<ApiResult<OrderDto>> UpdateOrderAsync(int id, UpdateOrderDto dto)
    {
        var req = Build(HttpMethod.Put, $"api/orders/{id}", JsonContent.Create(dto));
        return await SendAsync<OrderDto>(req);
    }

    public async Task<ApiResult<bool>> DeleteOrderAsync(int id)
    {
        var req = Build(HttpMethod.Delete, $"api/orders/{id}");
        return await SendAsync<bool>(req);
    }

    // ------------------ USERS ------------------

    public async Task<List<UserDto>?> GetUsersAsync()
    {
        var req = Build(HttpMethod.Get, "api/users");
        var res = await SendAsync<List<UserDto>>(req);
        return res.Value;
    }
}