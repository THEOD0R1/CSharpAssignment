using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Presentation.ConsoleApp.Dialogs;

IHost host = Host.CreateDefaultBuilder()
    .ConfigureServices((services) =>
        {
            services.AddSingleton<IJsonFileRepository, JsonFileRepository>();
            services.AddSingleton<IProductService, ProductService>();

            services.AddTransient<ProductDialogs>();
            services.AddTransient<MenuDialogs>();
        })
    .Build();

var menu = host.Services.GetRequiredService<MenuDialogs>();

menu.Run();