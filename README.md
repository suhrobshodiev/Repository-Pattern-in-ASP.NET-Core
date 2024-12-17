# How to Implement the Repository Pattern in ASP.NET Core Web API
## Introduction
The Repository Pattern is one of the key architectural solutions in software development. A repository creates an isolated layer between the domain model and data sources such as databases, files, and more.

## Core Principles of the Pattern
- **Abstraction over data sources**: The repository hides the implementation details of data storage.
- **Encapsulation of access operations**: All operations for reading, writing, updating, and deleting data are encapsulated within the repository.
- **Improved testability**: By using repository interfaces, it is easy to replace the actual data source with mock objects, simplifying the creation of unit tests.

## Pattern Structure
A repository is an interface that defines a standard set of methods for working with data:
- **Data retrieval**: Methods such as `findById`, `findAll`, and `findByCondition`.
- **Data creation/updating**: Methods such as `add`, `update`, and `save`.
- **Data deletion**: Methods such as `delete` and `deleteById`.

A concrete implementation of this interface can interact with a database, file system, or any other data source.

---

## Usage Example

### Defining the Domain Model
```csharp
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CatalogApi.Models;

public class Product
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    [BsonElement("name")] public string Name { get; set; } = null!;
    public required decimal Price { get; set; }
    public required string Category { get; set; } = null!;
    public required string Description { get; set; } = null!;
}
```

### Creating the Repository Interface
```csharp
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
```

### Implementing the Repository
The `ProductRepository` class implements the `IProductRepository` interface, ensuring all its defined methods and properties are provided.

```csharp
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
```

---

## Dependency Injection

The `CatalogController` gets an instance of `IProductRepository` through its constructor. This is called dependency injection. The controller doesn't depend on a specific implementation, such as `ProductRepository`, but instead relies on the `IProductRepository` interface.

This allows swapping out the actual repository implementation without changing the controller code.

```csharp
public CatalogController(IProductRepository repository)
{
    _repository = repository ?? throw new ArgumentNullException(nameof(repository));
}
```

---

## Using the Interface
The controller uses methods defined in `IProductRepository` to perform operations like getting, creating, updating, or deleting products.

### Example: CatalogController
```csharp
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

    [HttpPost]
    public async Task<IActionResult> CreateProductAsync(Product product)
    {
        await _repository.CreateProductAsync(product);
        return CreatedAtRoute("GetProductByIdAsync", new { id = product.Id }, product);
    }
}
```

---

## In Summary
The Repository Pattern is a powerful tool that simplifies data management. By abstracting data access and encapsulating operations, it enhances modularity, maintainability, and testability in software systems.

