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
    private bool _loaded = false;

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

    public async Task<ResponseResult<IReadOnlyList<Product>>> EnsureLoadedAsync()
    {
        if (_loaded)
            return ResponseResult<IReadOnlyList<Product>>.Ok(_products);

        StartNewCancellation();

        try
        {
            var result = await _jsonFileRepository.ReadAsync<Product>(_cts.Token);

            if (result.Success)
                _loaded = true;

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
    public async Task<ResponseResult<IReadOnlyList<Product>>> GetProductsAsync() 
    {
        await EnsureLoadedAsync();

        return ResponseResult<IReadOnlyList<Product>>.Ok(_products);
    }

    public async Task<ResponseResult> SaveProductAsync(ProductRequest productRequest)
    {
        StartNewCancellation();

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
            await _jsonFileRepository.WriteAsync<Product>(_products, _cts.Token);

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
            int alreadyExists = _products.FindIndex((product) => product.Name.Trim().Equals(updatedProduct.Name.Trim(), StringComparison.CurrentCultureIgnoreCase));

            if (alreadyExists != -1)
                return ResponseResult.Fail(409, "Product name is already taken.");

            var response = await _jsonFileRepository.UpdateAsync<Product>((product) => product.Id == updatedProduct.Id, updatedProduct, _cts.Token);

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
            var response = await _jsonFileRepository.DeleteAsync<Product>((product) => product.Id == productId, _cts.Token);

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

