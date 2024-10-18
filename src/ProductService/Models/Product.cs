namespace ProductService.Models;

public class Product
{
    public string Id { get; set; }
    public int Stock { get; set; }
    public string Description { get; set; }
    public List<string> Categories { get; set; }
    public decimal Price { get; set; }
}