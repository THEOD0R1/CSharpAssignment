using Infrastructure.Helpers.Generators;
using Infrastructure.Interfaces;
using Infrastructure.Models;
using Infrastructure.Repositories;

namespace Infrastructure.Services;

public class ProductService(IJsonFileRepository jsonFileRepository) : IProductService
{
    private List<Product> _products = [];
    private readonly IJsonFileRepository _jsonFileRepository = jsonFileRepository;
    private CancellationTokenSource? _cts;

    public void Cancel()
    {
        if (_cts == null)
            return;

        try
        {
            if (!_cts.IsCancellationRequested)
                _cts.Cancel();
        }
        catch (ObjectDisposedException)
        {
        }
        finally
        {
            try { _cts.Dispose(); } catch { };
            
            _cts = null;
        }
    }

    private void StartNewCancellation()
    {
        Cancel();
        _cts = new CancellationTokenSource();
    }

 
    public async Task<ResponseResult<IReadOnlyList<Product>>> GetProductsAsync() 
    {
        StartNewCancellation();

        try
        {
            var result = await _jsonFileRepository.ReadAsync<Product>(_cts!.Token);

            _products = result.Content!.ToList() ?? [];

            return result;

        }
        catch (Exception ex)
        {
            return ResponseResult<IReadOnlyList<Product>>.Fail(500, $"Failed to get products: {ex.Message}");
        }
        finally
        {
            Cancel();
        }

    }

    public async Task<ResponseResult> SaveProductAsync(ProductRequest productRequest)
    {
        StartNewCancellation();

        try
        {
            if (string.IsNullOrWhiteSpace(productRequest?.Name))
                return ResponseResult.Fail(400, "Product name is required.");

            int alreadyExists = _products.FindIndex((product) => product.Name.Trim().Equals(productRequest.Name.Trim(), StringComparison.CurrentCultureIgnoreCase));

            if (alreadyExists != -1)
                return ResponseResult.Fail(409, "Product name already exists.");
                
            var product = new Product
            {
                Id = IdGenerators.Product(),
                Name = productRequest.Name,
                Price = productRequest.Price,
                Category = productRequest.Category ?? new Category(),
                Manufacture = productRequest.Manufacture ?? new Manufacture(),
            };

            _products.Add(product);
            await _jsonFileRepository.WriteAsync<Product>(_products, _cts!.Token);

            return ResponseResult.Ok();
        }
        catch (Exception ex)
        {
            return ResponseResult.Fail(500, $"Failed to save product: {ex.Message}");
        }
        finally
        {
            Cancel();
        }
    }
    public async Task<ResponseResult> UpdateProductAsync(Product updatedProduct)
    {
        StartNewCancellation();

        try
        {
            if (string.IsNullOrWhiteSpace(updatedProduct?.Name))
                return ResponseResult.Fail(400, "Product name is required.");

            int alreadyExists = _products.FindIndex(product => string.Equals(product.Name?.Trim(), updatedProduct.Name?.Trim(), StringComparison.CurrentCultureIgnoreCase) && !object.Equals(product.Id, updatedProduct.Id)
);

            if (alreadyExists != -1)
                return ResponseResult.Fail(409, "Product name is already taken.");

            var response = await _jsonFileRepository.UpdateAsync<Product>((product) => product.Id == updatedProduct.Id, updatedProduct, _cts!.Token);

            var refreshList = await _jsonFileRepository.ReadAsync<Product>(_cts!.Token);

            _products = [.. refreshList.Content!];

            return response;
        }
        catch (Exception ex)
        {
            return ResponseResult.Fail(500, $"Failed to save update product: {ex.Message}");
        }
        finally
        {
            Cancel();
        }
    }

    public async Task<ResponseResult> DeleteProductAsync(string productId)
    {
        StartNewCancellation();

        try
        {
            var response = await _jsonFileRepository.DeleteAsync<Product>((product) => product.Id == productId, _cts!.Token);

            var refreshList = await _jsonFileRepository.ReadAsync<Product>(_cts!.Token);

            _products = [.. refreshList.Content!];

            return response;
        }
        catch (Exception ex)
        {
            return ResponseResult.Fail(500, $"Failed to delete product: {ex.Message}");
        }
        finally
        {
            Cancel();
        }
    }
}

