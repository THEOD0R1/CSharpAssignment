using CommunityToolkit.Mvvm.ComponentModel;

namespace Presentation.WpfApp.ViewModels;

public class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableObject _currentViewModel = null!;
}
