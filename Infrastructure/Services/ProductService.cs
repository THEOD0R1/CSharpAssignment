using Infrastructure.Helpers.Generators;
using Infrastructure.Interfaces;
using Infrastructure.Models;
using Infrastructure.Repositories;

namespace Infrastructure.Services;


public class ProductService(IJsonFileRepository jsonFileRepository) : IProductService
{
    private List<Product> _products = [];
    private readonly IJsonFileRepository _jsonFileRepository = jsonFileRepository;
    private bool _loaded = false;
    public async Task<ResponseResult<IReadOnlyList<Product>>> EnsureLoadedAsync(CancellationToken cancellationToken = default)
    {
        if (_loaded)
            return ResponseResult<IReadOnlyList<Product>>.Ok(_products);

        try
        {
            var result = await _jsonFileRepository.ReadAsync<Product>(cancellationToken);
            _products = result.Content!.ToList() ?? [];

            if (result.Success)
                _loaded = true;
            
            return result;
        }
        catch (Exception ex)
        {
            return ResponseResult<IReadOnlyList<Product>>.Fail(500, $"Failed to get products: {ex.Message}");
        }
    }
    public async Task<ResponseResult<IReadOnlyList<Product>>> GetProductsAsync(CancellationToken cancellationToken = default) 
    {
        await EnsureLoadedAsync(cancellationToken);

        return ResponseResult<IReadOnlyList<Product>>.Ok(_products);
    }

    public async Task<ResponseResult> SaveProductAsync(ProductRequest productRequest, CancellationToken cancellationToken = default)
    {
        try
        {
            int alreadyExists = _products.FindIndex((product) => product.Name.Trim().Equals(productRequest.Name.Trim(), StringComparison.CurrentCultureIgnoreCase));

            if (alreadyExists != -1)
                return ResponseResult.Fail(409, "Product name already exists.");

            var product = new Product
            {
                Id = IdGenerators.Product(),
                Name = productRequest.Name,
                Price = productRequest.Price,
            };

            _products.Add(product);
            await _jsonFileRepository.WriteAsync<Product>(_products, cancellationToken);

            return ResponseResult.Ok();
        }
        catch (Exception ex)
        {
            return ResponseResult.Fail(500, $"Failed to save product: {ex.Message}");
        }
    }
    public async Task<ResponseResult> UpdateProductAsync(ProductRequest productRequest, CancellationToken cancellationToken)
    {
        try
        {
           var response = await _jsonFileRepository.UpdateAsync<ProductRequest>((product) => product.Name == productRequest.Name, productRequest, cancellationToken);

           return response;
        }
        catch (Exception ex)
        {
            return ResponseResult.Fail(500, $"Failed to save update product: {ex.Message}");
        }
    }

    public async Task<ResponseResult> DeleteProductAsync(string productId, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _jsonFileRepository.DeleteAsync<Product>((product) => product.Id == productId, cancellationToken);

            return response;
        }
        catch (Exception ex)
        { 
            return ResponseResult.Fail(500, $"Failed to delete product: {ex.Message}");
        }

    }
}

