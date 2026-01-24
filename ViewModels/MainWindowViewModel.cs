using CommunityToolkit.Mvvm.ComponentModel;

namespace TP203.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private ViewModelBase _currentViewModel;

    public MainWindowViewModel()
    {
        _currentViewModel = new LoginViewModel(this);
    }

    public void NavigateToDashboard(string role)
    {
        // In a real app, we would pass the specific user context
        if (role == "Admin")
        {
            CurrentViewModel = new AdminDashboardViewModel(this);
        }
        else
        {
             CurrentViewModel = new MobileDashboardViewModel(this, role);
        }
    }
    
    public void Logout()
    {
        CurrentViewModel = new LoginViewModel(this);
    }
}
