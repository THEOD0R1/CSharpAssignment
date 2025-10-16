using Infrastructure.Helpers;
using Infrastructure.Interfaces;
using Infrastructure.Models;

namespace Presentation.ConsoleApp.Dialogs;

public class ProductDialogs(IProductService productService)
{
    private readonly IProductService _productService = productService;

    public async void AddProductDialog()
    {
        var productRequest = new ProductRequest();
        Console.Clear();

        Console.WriteLine("### NEW PRODUCT ###");
        Console.WriteLine("Product Name: ");
        string? name = Console.ReadLine()?.Trim();

        var validatedNameData = Validators.Name(name);

        if (!validatedNameData.IsSuccess)
        {
            Console.WriteLine(validatedNameData.Message);
            Console.WriteLine("Click anywhere to try again...");

            Console.ReadKey();

            AddProductDialog();
            return;
        }
        productRequest.Name = validatedNameData.Content!;

        Console.WriteLine("Product Price (SEK): ");
        bool isDecimal = decimal.TryParse(Console.ReadLine(), out decimal price);

        if (isDecimal == false)
        {
            Console.WriteLine("Failed to parse price. Please enter a valid price.");
            Console.WriteLine("Click anywhere to try again...");

            Console.ReadKey();

            AddProductDialog();
            return;
        }
        productRequest.Price = price;

        var saveProductResponse = await _productService.SaveProductAsync(productRequest);

        if (!saveProductResponse.Success)
        {
            Console.WriteLine(saveProductResponse.Error);
            Console.WriteLine("Click anywhere to try again...");

            Console.ReadKey();

            AddProductDialog();
            return;
        }

    }

    public async void ShowProductListDialog()
    {
        var products = await _productService.GetProductsAsync();

        Console.Clear();

        if (!products.Success)
        {
            Console.WriteLine(products?.Error);
            Console.WriteLine("Click anywhere to try again...");

            Console.ReadKey();
            ShowProductListDialog();

            return;
        }

        Console.WriteLine("### PRODUCT LIST ###");

        foreach (var product in products.Content!)
        {
            Console.WriteLine("Id: " + product.Id);
            Console.WriteLine("Name: " + product.Name);
            Console.WriteLine($"Price: {product.Price} SEK");
            Console.WriteLine("");
        }

        Console.WriteLine("Pres any key to continue...");
        Console.ReadKey();
    }
}
