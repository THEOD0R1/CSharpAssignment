namespace Infrastructure.Models;

public class ProductResult
{
    public bool Success { get; set; }
    public int StatusCode { get; set; }
    public string Error { get; set; } = null!;
}
