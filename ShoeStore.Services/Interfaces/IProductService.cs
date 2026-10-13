using ShoeStore.Services.Models;
using ShoeStoreData.Models;

namespace ShoeStore.Services.Interfaces;

public interface IProductService
{
    Task<PagedResult<Product>> GetProductsAsync(CatalogFilter filter);
    Task<Product?> GetProductByIdAsync(int id);
    Task<IEnumerable<ProductItem>> GetAvailableSizesAsync(int productId);
    Task<Product> CreateProductAsync(Product product);
    Task UpdateProductAsync(Product product);
    Task DeleteProductAsync(int id);
}