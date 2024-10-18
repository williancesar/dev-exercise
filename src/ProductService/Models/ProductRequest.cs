namespace ProductService.Models;

public class ProductRequest
{
    public string Description { get; set; }
    public List<Category> Categories { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
}