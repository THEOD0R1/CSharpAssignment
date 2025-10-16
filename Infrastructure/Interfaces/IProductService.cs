using Infrastructure.Models;

namespace Infrastructure.Interfaces;

public interface IProductService
{
    Task<ResponseResult> DeleteProductAsync(string productId, CancellationToken cancellationToken);
    Task<ResponseResult<IReadOnlyList<Product>>> GetProductsAsync(CancellationToken cancellationToken = default);
    Task<ResponseResult> SaveProductAsync(ProductRequest productRequest, CancellationToken cancellationToken = default);
    Task<ResponseResult> UpdateProductAsync(ProductRequest productRequest, CancellationToken cancellationToken);
}

