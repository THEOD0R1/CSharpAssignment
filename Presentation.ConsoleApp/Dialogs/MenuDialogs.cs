namespace Presentation.ConsoleApp.Dialogs;

public class MenuDialogs(ProductDialogs productDialogs)
{
    private readonly ProductDialogs _productDialogs = productDialogs;

    public void MenuOptionsDialog()
    {
        Console.Clear();
        Console.WriteLine("### MENU OPTIONS ###");
        Console.WriteLine("1. View Product List");
        Console.WriteLine("2. Add Product");
        Console.WriteLine("0. Exit Application");

        Console.WriteLine("Chose a menu option: ");
        var option = Console.ReadLine();

        switch (option)
        {
            case "1":
                _productDialogs.ShowProductListDialog(); 
                break;
            case "2":
                _productDialogs.AddProductDialog();
                break;
            case "0":
                Environment.Exit(0);
                break;
        }
    }

    public void Run()
    {
        while (true)
        {
            MenuOptionsDialog();
        }
    }
}
