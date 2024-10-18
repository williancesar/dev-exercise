using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;
using ProductService.Models;
using ProductService.Repositories;

namespace UnitTests;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _mockRepository;
    private readonly Mock<ILogger<ProductService.ProductService>> _mockLogger;
    private readonly ProductService.ProductService _productService;

    public ProductServiceTests()
    {
        _mockRepository = new Mock<IProductRepository>();
        _mockLogger = new Mock<ILogger<ProductService.ProductService>>();
        _productService = new ProductService.ProductService(_mockRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task CreateProduct_ShouldCreateProduct_WhenProductIsValid()
    {
        var product = new Product { Description = "New Product", Price = 19.99m, Stock = 100 };

        _mockRepository.Setup(r => r.FindAsync(It.IsAny<FilterDefinition<Product>>(), null, default))
            .ReturnsAsync(Mock.Of<IAsyncCursor<Product>>());

        await _productService.CreateProduct(product);

        _mockRepository.Verify(r => r.InsertOneAsync(product, null, default), Times.Once);
        _mockLogger.Verify(l => l.LogInformation(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task CreateProduct_ShouldThrowException_WhenProductDescriptionIsDuplicate()
    {
        // Arrange
        var product = new Product { Description = "Duplicate Product", Price = 19.99m, Stock = 100 };
        var existingProduct = new Product { Description = "Duplicate Product" };

        var mockCursor = new Mock<IAsyncCursor<Product>>();
        mockCursor.SetupSequence(c => c.MoveNext(It.IsAny<CancellationToken>()))
            .Returns(true)
            .Returns(false);
        mockCursor.Setup(c => c.Current).Returns(new[] { existingProduct });

        _mockRepository.Setup(r => r.FindAsync(It.IsAny<FilterDefinition<Product>>(), null, default))
            .ReturnsAsync(mockCursor.Object);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _productService.CreateProduct(product));
        _mockLogger.Verify(l => l.LogWarning(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task GetProductById_ShouldReturnProduct_WhenProductExists()
    {
        // Arrange
        var productId = "existing-id";
        var product = new Product { Id = productId, Description = "Existing Product", Price = 19.99m, Stock = 100 };

        var mockCursor = new Mock<IAsyncCursor<Product>>();
        mockCursor.SetupSequence(c => c.MoveNext(It.IsAny<CancellationToken>()))
            .Returns(true)
            .Returns(false);
        mockCursor.Setup(c => c.Current).Returns(new[] { product });

        _mockRepository.Setup(r => r.FindAsync(It.IsAny<FilterDefinition<Product>>(), null, default))
            .ReturnsAsync(mockCursor.Object);

        // Act
        var result = await _productService.GetProductById(productId);

        // Assert
        Assert.Equal(productId, result.Id);
        _mockLogger.Verify(l => l.LogInformation(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task GetProductById_ShouldThrowException_WhenProductDoesNotExist()
    {
        // Arrange
        var productId = "non-existing-id";

        var mockCursor = new Mock<IAsyncCursor<Product>>();
        mockCursor.SetupSequence(c => c.MoveNext(It.IsAny<CancellationToken>()))
            .Returns(false);

        _mockRepository.Setup(r => r.FindAsync(It.IsAny<FilterDefinition<Product>>(), null, default))
            .ReturnsAsync(mockCursor.Object);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _productService.GetProductById(productId));
        _mockLogger.Verify(l => l.LogWarning(It.IsAny<string>()), Times.Once);
    }
}