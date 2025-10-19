using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Infrastructure.Interfaces;
using Infrastructure.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Presentation.WpfApp.ViewModels;

public partial class ProductEditViewModel(IServiceProvider serviceProvider) : ObservableObject
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    [ObservableProperty]
    private string _pageTitle = "EDIT PRODUCT";

    [ObservableProperty]
    private Product _productToUpdate = new()
    {
        Category = new Category(),
        Manufacture = new Manufacture()
    };


    [RelayCommand]
    private async Task UpdateProduct()
    {
        var ps = _serviceProvider.GetRequiredService<IProductService>();
        var response = await ps.UpdateProductAsync(ProductToUpdate);

        if (!response.Success)
            return;

        GoToProductList();
    }

    [RelayCommand]
    private void CancelProductUpdate()
    {
        GoToProductList();
    }

    private void GoToProductList()
    {
        var plvm = _serviceProvider.GetRequiredService<ProductListViewModel>();
        var mvn = _serviceProvider.GetRequiredService<MainViewModel>();

        mvn.CurrentViewModel = plvm;
    }

}
