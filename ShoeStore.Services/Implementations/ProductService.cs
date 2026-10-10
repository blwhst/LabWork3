using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShoeStore.Services.Interfaces;
using ShoeStore.Services.Models;
using ShoeStore.Services.Validators;
using ShoeStoreData.Contexts;
using ShoeStoreData.Models;
using ShoeStoreException;

namespace ShoeStore.Services.Implementations;

public class ProductService : IProductService
{
    private readonly AppDbContext _context;
    private readonly ILogger<ProductService> _logger;

    public ProductService(AppDbContext context, ILogger<ProductService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PagedResult<Product>> GetProductsAsync(CatalogFilter filter)
    {
        _logger.LogInformation("Запрошен каталог товаров. Фильтр: {@Filter}", filter);

        var query = _context.Products
            .Include(p => p.Subcategory)
                .ThenInclude(s => s.Category)
            .Include(p => p.Materials)
            .Include(p => p.ProductItems)
            .AsQueryable();

        if (filter.CategoryId.HasValue)
            query = query.Where(p => p.Subcategory.CategoryId == filter.CategoryId.Value);

        if (filter.SubcategoryId.HasValue)
            query = query.Where(p => p.SubcategoryId == filter.SubcategoryId.Value);

        if (!string.IsNullOrWhiteSpace(filter.Manufacturer))
            query = query.Where(p => p.Manufacturer.Contains(filter.Manufacturer));

        if (!string.IsNullOrWhiteSpace(filter.SearchQuery))
            query = query.Where(p => p.Name.Contains(filter.SearchQuery) ||
                                     (p.Description != null && p.Description.Contains(filter.SearchQuery)));

        query = filter.SortBy switch
        {
            "price_asc" => query.OrderBy(p => p.Price),
            "price_desc" => query.OrderByDescending(p => p.Price),
            "name_asc" => query.OrderBy(p => p.Name),
            "name_desc" => query.OrderByDescending(p => p.Name),
            _ => query.OrderBy(p => p.ProductId)
        };

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return new PagedResult<Product>
        {
            Items = items,
            TotalCount = totalCount,
            Page = filter.Page,
            PageSize = filter.PageSize
        };
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        _logger.LogInformation("Поиск товара по Id: {Id}", id);

        var product = await _context.Products
            .Include(p => p.Materials)
            .Include(p => p.ProductItems)
            .Include(p => p.Subcategory)
                .ThenInclude(s => s.Category)
            .FirstOrDefaultAsync(p => p.ProductId == id);

        if (product == null)
        {
            throw Exceptions.ProductNotFound(id);
        }

        return product;
    }

    public async Task<IEnumerable<ProductItem>> GetAvailableSizesAsync(int productId)
    {
        return await _context.ProductItems
            .Where(pi => pi.ProductId == productId && pi.Quantity > 0)
            .ToListAsync();
    }

    public async Task<Product> CreateProductAsync(Product product)
    {
        ProductValidator.Validate(product);

        _context.Products.Add(product);

        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("Создан новый товар: {Name} (ID: {Id})",
                product.Name, product.ProductId);
            return product;
        }
        catch (Exception ex)
        {
            throw Exceptions.SaveChanges(ex);
        }
    }

    public async Task UpdateProductAsync(Product product)
    {
        ProductValidator.Validate(product);

        var existing = await _context.Products
            .Include(p => p.Materials)
            .FirstOrDefaultAsync(p => p.ProductId == product.ProductId);

        if (existing == null)
            throw Exceptions.ProductNotFound(product.Name, product.Manufacturer);

        existing.Name = product.Name;
        existing.Price = product.Price;
        existing.Description = product.Description;
        existing.Manufacturer = product.Manufacturer;
        existing.SubcategoryId = product.SubcategoryId;
        existing.Image = product.Image;

        if (product.Materials != null && product.Materials.Any())
        {
            existing.Materials.Clear();
            var materialIds = product.Materials.Select(m => m.MaterialId).ToList();
            var materials = await _context.Materials
                .Where(m => materialIds.Contains(m.MaterialId))
                .ToListAsync();

            foreach (var m in materials)
                existing.Materials.Add(m);
        }

        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("Обновлён товар ID: {Id}", product.ProductId);
        }
        catch (Exception ex)
        {
            throw Exceptions.SaveChanges(ex);
        }
    }

    public async Task DeleteProductAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
            throw Exceptions.ProductNotFound(id);

        _context.Products.Remove(product);

        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("Удалён товар ID: {Id}", id);
        }
        catch (Exception ex)
        {
            throw Exceptions.SaveChanges(ex);
        }
    }
}