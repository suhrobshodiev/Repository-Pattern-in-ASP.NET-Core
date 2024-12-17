using CatalogApi.Models;

namespace CatalogApi.Repositories;

public interface IProductRepository
{
    Task<List<Product>> GetProductsAsync();
    Task<Product?> GetProductByIdAsync(string id);
    Task CreateProductAsync(Product product);
    Task<bool> UpdateProductAsync(Product product);
    Task<bool> RemoveProductAsync(string id);
}