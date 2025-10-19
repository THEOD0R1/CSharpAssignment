namespace Infrastructure.Models;

public class ProductRequest
{
    public string Name { get; set; } = null!;
    public decimal? Price { get; set; }
    public Category Category { get; set; } = null!;
    public Manufacture Manufacture { get; set; } = null!;
}
