using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace TP203.ViewModels;

public partial class MobileDashboardViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainViewModel;
    
    [ObservableProperty]
    private string _userRole;

    [ObservableProperty]
    private string _title = "Mon Planning";

    public MobileDashboardViewModel(MainWindowViewModel mainViewModel, string role)
    {
        _mainViewModel = mainViewModel;
        UserRole = role;
        Title = $"{role} Dashboard";
    }

    [RelayCommand]
    public void Logout()
    {
        _mainViewModel.Logout();
    }
}
