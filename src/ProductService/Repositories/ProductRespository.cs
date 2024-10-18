
using MongoDB.Driver;
using ProductService.Models;

namespace ProductService.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly IMongoCollection<Product> _collection;

    public ProductRepository(IMongoClient mongoClient)
    {
        var database = mongoClient.GetDatabase("ProductDb");
        _collection = database.GetCollection<Product>("Products");
    }

    public Task<IAsyncCursor<Product>> FindAsync(FilterDefinition<Product> filter, FindOptions<Product, Product> options = null, CancellationToken cancellationToken = default)
    {
        return _collection.FindAsync(filter, options, cancellationToken);
    }

    public Task InsertOneAsync(Product product, InsertOneOptions options = null, CancellationToken cancellationToken = default)
    {
        return _collection.InsertOneAsync(product, options, cancellationToken);
    }
}