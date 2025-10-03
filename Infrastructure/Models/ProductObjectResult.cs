namespace Infrastructure.Models;

public class ProductObjectResult<T> : ProductResult
{
    public T? Content { get; set; }
}