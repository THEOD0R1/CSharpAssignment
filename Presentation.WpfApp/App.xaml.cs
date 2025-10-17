using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Presentation.WpfApp.ViewModels;
using System.Windows;

namespace Presentation.WpfApp;


public partial class App : Application
{
    IHost host;

    public App()
    {
        host = Host.CreateDefaultBuilder().ConfigureServices(services =>
        {
            services.AddSingleton<IJsonFileRepository, JsonFileRepository>();
            services.AddSingleton<IProductService, ProductService>();

            services.AddSingleton<MainViewModel>();
            services.AddSingleton<MainWindow>();

            services.AddTransient<ProductListViewModel>();

            services.AddTransient<ProductAddViewModel>();

        })
        .Build();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var plvm = host.Services.GetRequiredService<ProductListViewModel>();
        await plvm.PopulateProductListAsync();

        var mvm = host.Services.GetRequiredService<MainViewModel>();
        mvm.CurrentViewModel = plvm;

        var window = host.Services.GetRequiredService<MainWindow>();
        window.DataContext = mvm;

        window.Show();
    }
}
