using MongoDB.Driver;
using ProductService.Models;

namespace ProductService.Repositories;

public interface IProductRepository
{
    Task<IAsyncCursor<Product>> FindAsync(FilterDefinition<Product> filter,
        FindOptions<Product, Product> options = null, CancellationToken cancellationToken = default);
    
    Task InsertOneAsync(Product product, InsertOneOptions options = null, CancellationToken cancellationToken = default);
}