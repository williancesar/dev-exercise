using MongoDB.Driver;
using ProductService.Models;
using ProductService.Repositories;

namespace ProductService;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;
    private readonly ILogger<ProductService> _logger;

    public ProductService(IProductRepository repository, ILogger<ProductService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task CreateProduct(Product product)
    {
        try
        {
            var existingProducts = await _repository.FindAsync(Builders<Product>.Filter.Eq(p => p.Description, product.Description));
            if (await existingProducts.AnyAsync())
            {
                _logger.LogWarning($"Duplicate product description: {product.Description}");
                
                throw new InvalidOperationException("Product with this description already exists.");
            }

            product.Id = Guid.NewGuid().ToString();
            
            await _repository.InsertOneAsync(product);
            
            _logger.LogInformation($"Product {product.Id} created successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error creating product: {ex.Message}");
            
            throw new Exception("An error occurred while creating the product.");
        }
    }

    public async Task<Product> GetProductById(string id)
    {
        try
        {
            var products = await _repository.FindAsync(Builders<Product>.Filter.Eq(p => p.Id, id));
            var product = await products.FirstOrDefaultAsync();
            if (product == null)
            {
                _logger.LogWarning($"Product with ID {id} not found.");
                
                throw new KeyNotFoundException("Product not found.");
            }
            
            _logger.LogInformation($"Product {id} retrieved successfully.");
            
            return product;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving product: {ex.Message}");
            
            throw new Exception("An error occurred while retrieving the product.");
        }
    }
}