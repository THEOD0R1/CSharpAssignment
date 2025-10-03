namespace Infrastructure.Models;

public class ProductObjectResult<T> : ResponseResult
{
    public T? Content { get; set; }
}