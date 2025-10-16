using Infrastructure.Models;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Moq;

namespace Infrastructure.Tests.Services;

// Created by ChatGPT
public class ProductService_Tests
{
    private readonly Mock<IJsonFileRepository> _jsonRepoMock;
    private readonly ProductService _service;

    public ProductService_Tests()
    {
        _jsonRepoMock = new Mock<IJsonFileRepository>();
        _service = new ProductService(_jsonRepoMock.Object);
    }

    [Fact]
    public async Task GetProductsAsync_ReturnsProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new() { Id = "1", Name = "Test", Price = 10 }
        };
        _jsonRepoMock.Setup(r => r.ReadAsync<Product>(It.IsAny<CancellationToken>()))
            .ReturnsAsync(ResponseResult<IReadOnlyList<Product>>.Ok(products));

        // Act
        var result = await _service.GetProductsAsync();

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Content);
        Assert.Single(result.Content);
        Assert.Equal("Test", result.Content[0].Name);
    }

    [Fact]
    public async Task SaveProductAsync_DuplicateName_ReturnsConflict()
    {
        // Arrange
        var products = new List<Product>
        {
            new() { Id = "1", Name = "Test", Price = 10 }
        };
        _jsonRepoMock.Setup(r => r.ReadAsync<Product>(It.IsAny<CancellationToken>()))
            .ReturnsAsync(ResponseResult<IReadOnlyList<Product>>.Ok(products));
        await _service.GetProductsAsync();

        var request = new ProductRequest { Name = "Test", Price = 20 };

        // Act
        var result = await _service.SaveProductAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(409, result.StatusCode);
    }

    [Fact]
    public async Task SaveProductAsync_NewProduct_Succeeds()
    {
        // Arrange
        _jsonRepoMock.Setup(r => r.ReadAsync<Product>(It.IsAny<CancellationToken>()))
            .ReturnsAsync(ResponseResult<IReadOnlyList<Product>>.Ok([]));
        _jsonRepoMock.Setup(r => r.WriteAsync(It.IsAny<IEnumerable<Product>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ResponseResult.Ok());

        var request = new ProductRequest { Name = "NewProduct", Price = 99 };

        // Act
        var result = await _service.SaveProductAsync(request);

        // Assert
        Assert.True(result.Success);
    }
}