using MongoDB.Driver;
using ProductService;
using ProductService.Models;
using ProductService.Repositories;
using ProductService.Validators;

var builder = WebApplication.CreateBuilder(args);

// MongoDB setup
var mongoConnectionString = Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING") ?? "mongodb://localhost:27017";
var mongoClient = new MongoClient(mongoConnectionString);

// Register services
builder.Services.AddSingleton<IMongoClient>(mongoClient);
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService.ProductService>();

var app = builder.Build();

app.MapPost("/products", async (ProductRequest productRequest, IProductService productService) =>
{
    // Validate product description
    if (!Validator.IsValidDescription(productRequest.Description))
    {
        return Results.Problem("Product description is required.", statusCode: 400);
    }

    if (!Validator.IsValidPrice(productRequest.Price))
    {
        return Results.Problem("Price must be a valid positive number.", statusCode: 400);
    }

    if (!Validator.IsValidStock(productRequest.Stock))
    {
        return Results.Problem("Stock must be a non-negative integer.", statusCode: 400);
    }

    if (!Validator.AreValidCategories(productRequest.Categories))
    {
        return Results.Problem("Categories must have valid UUIDs and names.", statusCode: 400);
    }

    var product = new Product
    {
        Description = productRequest.Description,
        Categories = productRequest.Categories.Select(c => c.Id).ToList(),
        Price = productRequest.Price,
        Stock = productRequest.Stock
    };

    try
    {
        await productService.CreateProduct(product);
        
        return Results.Created($"/products/{product.Id}", product);
    }
    catch (InvalidOperationException)
    {
        return Results.Problem("Product with this description already exists.", statusCode: 409);
    }
    catch (Exception)
    {
        return Results.Problem("An error occurred while creating the product.", statusCode: 500);
    }
});

app.MapGet("/products/{id}", async (string id, IProductService productService) =>
{
    if (!Validator.IsValidUUID(id))
    {
        return Results.Problem("Invalid product ID format.", statusCode: 400);
    }

    try
    {
        var product = await productService.GetProductById(id);
        
        return Results.Ok(product);
    }
    catch (KeyNotFoundException)
    {
        return Results.Problem("Product not found.", statusCode: 404);
    }
    catch (Exception)
    {
        return Results.Problem("An error occurred while creating the product.", statusCode: 500);
    }
});

app.Run();