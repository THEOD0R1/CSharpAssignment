namespace Infrastructure.Models;

public class Product
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public decimal?  Price { get; set; }
    public Category Category { get; set; } = null!;
    public Manufacture Manufacture { get; set; } = null!;
}
