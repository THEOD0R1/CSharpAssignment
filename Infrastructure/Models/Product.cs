namespace Infrastructure.Models;

public class Product
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; } 
    public decimal Price { get; set; }
}
