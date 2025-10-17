using Infrastructure.Models;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Moq;

namespace Infrastructure.Tests.Services;

/* ############### Created by ChatGPT ###############
*
* This file contains unit tests for the ProductService class.
* It validates the behavior of product loading, saving, updating,
* and deleting operations using a mocked IJsonFileRepository.
* 
* TEST OVERVIEW:
* ------------------------------------------------------
* 1. EnsureLoadedAsync_ReturnsCachedProducts_WhenAlreadyLoaded
*     Verifies that if products are already loaded, it returns them directly without reloading.
*
* 2. EnsureLoadedAsync_LoadsProducts_FromRepository
*     Ensures that the service correctly loads product data from the repository.
*
* 3. GetProductsAsync_ReturnsLoadedProducts
*     Verifies that GetProductsAsync returns products after EnsureLoadedAsync is called.
*
* 4. SaveProductAsync_NewProduct_Succeeds
*     Confirms that saving a unique product adds it successfully and writes it to the repository.
*
* 5. SaveProductAsync_DuplicateName_ReturnsConflict
*     Ensures that saving a product with an existing name returns a 409 conflict.
*
* 6. UpdateProductAsync_NewName_Succeeds
*     Verifies that updating a product with a unique name succeeds.
*
* 7. UpdateProductAsync_DuplicateName_ReturnsConflict
*     Ensures that updating a product to a name that already exists returns a conflict.
*
* 8. DeleteProductAsync_Succeeds
*     Confirms that deleting a product calls the repository and succeeds.
*
* 9. Cancel_DisposesTokenSourceSafely
*     Verifies that calling Cancel cleans up and disposes the CancellationTokenSource.
*     
* ############################################################
*/
public class ProductService_Tests
{
    private readonly Mock<IJsonFileRepository> _repoMock;
    private readonly ProductService _service;

    public ProductService_Tests()
    {
        _repoMock = new Mock<IJsonFileRepository>();
        _service = new ProductService(_repoMock.Object);
    }

    [Fact]
    public async Task EnsureLoadedAsync_ReturnsCachedProducts_WhenAlreadyLoaded()
    {
        // Arrange
        await _service.EnsureLoadedAsync();
        var products = new List<Product> { new() { Id = "1", Name = "Cached", Price = 1 } };
        typeof(ProductService)
            .GetField("_products", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .SetValue(_service, products);
        typeof(ProductService)
            .GetField("_loaded", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .SetValue(_service, true);

        // Act
        var result = await _service.EnsureLoadedAsync();

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Cached", result.Content![0].Name);
    }

    [Fact]
    public async Task EnsureLoadedAsync_LoadsProducts_FromRepository()
    {
        // Arrange
        var products = new List<Product> { new() { Id = "1", Name = "Loaded", Price = 10 } };
        _repoMock.Setup(r => r.ReadAsync<Product>(It.IsAny<CancellationToken>()))
                 .ReturnsAsync(ResponseResult<IReadOnlyList<Product>>.Ok(products));

        // Act
        var result = await _service.EnsureLoadedAsync();

        // Assert
        Assert.True(result.Success);
        Assert.Single(result.Content!);
        Assert.Equal("Loaded", result.Content![0].Name);
    }

    [Fact]
    public async Task GetProductsAsync_ReturnsLoadedProducts()
    {
        // Arrange
        var products = new List<Product> { new() { Id = "2", Name = "TestProduct", Price = 25 } };
        _repoMock.Setup(r => r.ReadAsync<Product>(It.IsAny<CancellationToken>()))
                 .ReturnsAsync(ResponseResult<IReadOnlyList<Product>>.Ok(products));

        // Act
        var result = await _service.GetProductsAsync();

        // Assert
        Assert.True(result.Success);
        Assert.Single(result.Content!);
        Assert.Equal("TestProduct", result.Content![0].Name);
    }

    [Fact]
    public async Task SaveProductAsync_NewProduct_Succeeds()
    {
        // Arrange
        _repoMock.Setup(r => r.ReadAsync<Product>(It.IsAny<CancellationToken>()))
                 .ReturnsAsync(ResponseResult<IReadOnlyList<Product>>.Ok([]));
        _repoMock.Setup(r => r.WriteAsync(It.IsAny<IEnumerable<Product>>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(ResponseResult.Ok());

        var request = new ProductRequest { Name = "New", Price = 99 };

        // Act
        await _service.EnsureLoadedAsync();
        var result = await _service.SaveProductAsync(request);

        // Assert
        Assert.True(result.Success);
        _repoMock.Verify(r => r.WriteAsync(It.IsAny<IEnumerable<Product>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SaveProductAsync_DuplicateName_ReturnsConflict()
    {
        // Arrange
        var products = new List<Product> { new() { Id = "1", Name = "Duplicate", Price = 10 } };
        _repoMock.Setup(r => r.ReadAsync<Product>(It.IsAny<CancellationToken>()))
                 .ReturnsAsync(ResponseResult<IReadOnlyList<Product>>.Ok(products));
        await _service.EnsureLoadedAsync();

        var request = new ProductRequest { Name = "Duplicate", Price = 50 };

        // Act
        var result = await _service.SaveProductAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(409, result.StatusCode);
    }

    [Fact]
    public async Task UpdateProductAsync_NewName_Succeeds()
    {
        // Arrange
        _repoMock.Setup(r => r.UpdateAsync<Product>(It.IsAny<Func<Product, bool>>(), It.IsAny<Product>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(ResponseResult.Ok());
        typeof(ProductService)
            .GetField("_products", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .SetValue(_service, new List<Product>());

        var updated = new Product { Id = "1", Name = "Updated", Price = 5 };

        // Act
        var result = await _service.UpdateProductAsync(updated);

        // Assert
        Assert.True(result.Success);
    }

    [Fact]
    public async Task UpdateProductAsync_DuplicateName_ReturnsConflict()
    {
        // Arrange
        var products = new List<Product> { new() { Id = "2", Name = "Existing", Price = 1 } };
        typeof(ProductService)
            .GetField("_products", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .SetValue(_service, products);

        var updated = new Product { Id = "3", Name = "Existing", Price = 5 };

        // Act
        var result = await _service.UpdateProductAsync(updated);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(409, result.StatusCode);
    }

    [Fact]
    public async Task DeleteProductAsync_Succeeds()
    {
        // Arrange
        _repoMock.Setup(r => r.DeleteAsync<Product>(It.IsAny<Func<Product, bool>>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(ResponseResult.Ok());

        // Act
        var result = await _service.DeleteProductAsync("1");

        // Assert
        Assert.True(result.Success);
        _repoMock.Verify(r => r.DeleteAsync<Product>(It.IsAny<Func<Product, bool>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public void Cancel_DisposesTokenSourceSafely()
    {
        // Arrange
        var ctsField = typeof(ProductService).GetField("_cts", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
        var cts = new CancellationTokenSource();
        ctsField.SetValue(_service, cts);

        // Act
        _service.Cancel();

        // Assert
        Assert.Null(ctsField.GetValue(_service));
    }
}
