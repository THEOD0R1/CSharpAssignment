using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Infrastructure.Interfaces;
using Infrastructure.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;

namespace Presentation.WpfApp.ViewModels;

public partial class ProductListViewModel : ObservableObject
{
    private readonly IServiceProvider _serviceProvider;

    public ProductListViewModel(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _ = PopulateProductListAsync();
    }

    [ObservableProperty]
    private ObservableCollection<Product> _products = [];

    [ObservableProperty]
    private string _pageTitle = "VIEW PRODUCTS";

    [RelayCommand]
    private void GoToAddProduct()
    {
        var pavm = _serviceProvider.GetRequiredService<ProductAddViewModel>();
        var mvn = _serviceProvider.GetRequiredService<MainViewModel>();

        mvn.CurrentViewModel = pavm;
    }
    [RelayCommand]
    private void GoToEditProduct(Product product)
    {
        var pevm = _serviceProvider.GetRequiredService<ProductEditViewModel>();
        pevm.ProductToUpdate = new Product
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Category = product.Category,
            Manufacture = product.Manufacture,
        };

        var mvn = _serviceProvider.GetRequiredService<MainViewModel>();
        mvn.CurrentViewModel = pevm;
    }

    [RelayCommand]
    private async Task DeleteProduct(string id)
    {
        var ps = _serviceProvider.GetRequiredService<IProductService>();

        await ps.DeleteProductAsync(id);

        await PopulateProductListAsync();
    }

    public async Task PopulateProductListAsync()
    {
        var ps = _serviceProvider.GetRequiredService<IProductService>();

        var products = await ps.GetProductsAsync();

        Products = new ObservableCollection<Product>(products.Content!);
    }
}
