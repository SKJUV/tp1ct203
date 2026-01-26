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
        CurrentViewModel = role switch
        {
            "Admin" => new AdminDashboardViewModel(this),
            "Enseignant" => new TeacherDashboardViewModel(this),
            "Etudiant" => new StudentDashboardViewModel(this),
            "Delegue" => new MobileDashboardViewModel(this, role),
            _ => new LoginViewModel(this) // Fallback to login if unknown
        };
    }
    
    public void Logout()
    {
        CurrentViewModel = new LoginViewModel(this);
    }
}
