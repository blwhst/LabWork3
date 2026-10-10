namespace ShoeStore.Services.Models;

public class CatalogFilter
{
    public int? CategoryId { get; set; }
    public int? SubcategoryId { get; set; }
    public string? Manufacturer { get; set; }
    public string? SearchQuery { get; set; }    public string? SortBy { get; set; }    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}