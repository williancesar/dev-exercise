using ProductService.Models;

namespace ProductService;

public interface IProductService
{
    Task CreateProduct(Product product);
    
    Task<Product> GetProductById(string id);
}