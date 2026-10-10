using ShoeStoreData.Models;

namespace ShoeStore.Services.Interfaces;

public interface IReferenceService
{
    Task<IEnumerable<Category>> GetCategoriesAsync();
    Task<IEnumerable<Subcategory>> GetSubcategoriesAsync(int categoryId);
    Task<IEnumerable<Material>> GetMaterialsAsync();
}