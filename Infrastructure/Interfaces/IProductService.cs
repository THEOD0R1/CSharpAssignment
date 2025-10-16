using Infrastructure.Models;

namespace Infrastructure.Interfaces;

public interface IProductService
{
    void Cancel();
    Task<ResponseResult> DeleteProductAsync(string productId);
    Task<ResponseResult<IReadOnlyList<Product>>> GetProductsAsync();
    Task<ResponseResult> SaveProductAsync(ProductRequest productRequest);
    Task<ResponseResult> UpdateProductAsync(Product updatedProduct);
}

