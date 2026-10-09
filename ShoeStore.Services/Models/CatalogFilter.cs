namespace ShoeStore.Services.Models;

public class CatalogFilter
{
    public int? CategoryId { get; set; }
    public int? SubcategoryId { get; set; }
    public string? Manufacturer { get; set; }
    public string? SearchQuery { get; set; } // поиск по названию и описанию
    public string? SortBy { get; set; } // "price_asc", "price_desc", "name_asc", "name_desc"
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}