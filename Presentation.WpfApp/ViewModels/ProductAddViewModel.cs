using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Infrastructure.Interfaces;
using Infrastructure.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Presentation.WpfApp.ViewModels;

public partial class ProductAddViewModel(IServiceProvider serviceProvider) : ObservableObject
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    [ObservableProperty]
    private string _pageTitle = "ADD PRODUCT";

    [ObservableProperty]
    private ProductRequest _newProduct = new()
    {
        Manufacture = new(),
        Category = new()
    };

    [RelayCommand]
    private async Task SaveNewProduct()
    {
        var ps = _serviceProvider.GetRequiredService<IProductService>();
        await ps.SaveProductAsync(NewProduct);

        GoToProductList();
    }

    [RelayCommand]
    private void CancelNewProduct()
    {
        GoToProductList();
    }

    private void GoToProductList()
    {
        var plvm= _serviceProvider.GetRequiredService<ProductListViewModel>();
        var mvn = _serviceProvider.GetRequiredService<MainViewModel>();

        mvn.CurrentViewModel = plvm;
    }
}
