using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShoeStore.Services.Interfaces;
using ShoeStoreData.Contexts;
using ShoeStoreData.Models;
using ShoeStoreException;

namespace ShoeStore.Services.Implementations;

public class ReferenceService : IReferenceService
{
    private readonly AppDbContext _context;
    private readonly ILogger<ReferenceService> _logger;

    public ReferenceService(AppDbContext context, ILogger<ReferenceService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<Category>> GetCategoriesAsync()
    {
        _logger.LogInformation("Запрошен список всех категорий");
        return await _context.Categories.ToListAsync();
    }

    public async Task<IEnumerable<Subcategory>> GetSubcategoriesAsync(int categoryId)
    {
        _logger.LogInformation("Запрошены подкатегории для категории Id: {CategoryId}", categoryId);

        var exists = await _context.Categories.AnyAsync(c => c.CategoryId == categoryId);
        if (!exists)
            throw Exceptions.CategoryNotFound(categoryId);

        return await _context.Subcategories
            .Where(s => s.CategoryId == categoryId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Material>> GetMaterialsAsync()
    {
        _logger.LogInformation("Запрошен список всех материалов");
        return await _context.Materials.ToListAsync();
    }
}