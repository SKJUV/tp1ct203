using CommunityToolkit.Mvvm.ComponentModel;
using TP203.Models;

namespace TP203.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private ViewModelBase _currentViewModel;

    public MainWindowViewModel()
    {
        _currentViewModel = new LoginViewModel(this);
    }

    public void NavigateToDashboard(User user)
    {
        string role = user.Role.ToString();
        // Fallback / Patch for Type 0 or unknown
        if (user.Type == 0 || role == "0") role = "Admin";

        CurrentViewModel = role switch
        {
            "Admin" => new AdminDashboardViewModel(this),
            "Enseignant" => new TeacherDashboardViewModel(this, user),
            "Etudiant" => new StudentDashboardViewModel(this, user),
            "Delegue" => new MobileDashboardViewModel(this, user),
            _ => new LoginViewModel(this) // Fallback to login if unknown
        };
    }
    
    public void Logout()
    {
        CurrentViewModel = new LoginViewModel(this);
    }

    public void NavigateToRegister()
    {
        CurrentViewModel = new RegisterViewModel(this);
    }
}
