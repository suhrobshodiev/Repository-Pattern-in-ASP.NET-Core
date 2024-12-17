using CatalogApi.Context;
using CatalogApi.Models;
using MongoDB.Driver;

namespace CatalogApi.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ICatalogContext _context;

    public ProductRepository(ICatalogContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<List<Product>> GetProductsAsync()
    {
        return await _context.Products.Find(_ => true).ToListAsync();
    }

    public async Task<Product?> GetProductByIdAsync(string id)
    {
        return await _context.Products.Find(p => p.Id == id).FirstOrDefaultAsync();
    }

    public async Task CreateProductAsync(Product product)
    {
        await _context.Products.InsertOneAsync(product);
    }

    public async Task<bool> UpdateProductAsync(Product product)
    {
        return (await _context.Products.ReplaceOneAsync(p => p.Id == product.Id, product)).IsAcknowledged;
    }

    public async Task<bool> RemoveProductAsync(string id)
    {
        return (await _context.Products.DeleteOneAsync(p => p.Id == id)).IsAcknowledged;
    }
}