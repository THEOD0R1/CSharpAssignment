using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Infrastructure.Interfaces;
using Infrastructure.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;

namespace Presentation.WpfApp.ViewModels;

public partial class ProductListViewModel(IServiceProvider serviceProvider) : ObservableObject
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    [ObservableProperty]
    private ObservableCollection<Product> _products = [];

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
        pevm.ProductToUpdate = product;

        var mvn = _serviceProvider.GetRequiredService<MainViewModel>();
        mvn.CurrentViewModel = pevm;
    }

    public async Task PopulateProductListAsync()
    {
        var ps = _serviceProvider.GetRequiredService<IProductService>();

        var products = await ps.GetProductsAsync();

        Products = [.. products.Content!];
    }
}
