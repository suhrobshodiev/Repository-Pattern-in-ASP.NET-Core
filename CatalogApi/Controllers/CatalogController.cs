using CatalogApi.Models;
using CatalogApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CatalogApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CatalogController : ControllerBase
{
    private readonly IProductRepository _repository;

    public CatalogController(IProductRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    [HttpGet]
    public async Task<List<Product>> GetProductsAsync()
    {
        return await _repository.GetProductsAsync();
    }

    [HttpGet("{id:length(24)}", Name = "GetProductByIdAsync")]
    public async Task<IActionResult> GetProductByIdAsync(string id)
    {
        var product = await _repository.GetProductByIdAsync(id);
        if (product == null) return NotFound();
        return Ok(product);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProductAsync(Product product)
    {
        await _repository.CreateProductAsync(product);
        return CreatedAtRoute("GetProductByIdAsync", new { id = product.Id }, product);
    }

    [HttpPut("{id:length(24)}")]
    public async Task<IActionResult> UpdateProductAsync(Product product)
    {
        return Ok(await _repository.UpdateProductAsync(product));
    }

    [HttpDelete("{id:length(24)}")]
    public async Task<IActionResult> DeleteProductAsync(string id)
    {
        return Ok(await _repository.RemoveProductAsync(id));
    }
}