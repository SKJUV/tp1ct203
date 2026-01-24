using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace TP203.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainViewModel;

    [ObservableProperty]
    private string _username = "";

    [ObservableProperty]
    private string _password = "";

    [ObservableProperty]
    private string _errorMessage = "";

    public LoginViewModel(MainWindowViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
    }

    [RelayCommand]
    public void LoginAsAdmin()
    {
        // Simulate Login
        _mainViewModel.NavigateToDashboard("Admin");
    }

    [RelayCommand]
    public void LoginAsTeacher()
    {
        _mainViewModel.NavigateToDashboard("Teacher");
    }

    [RelayCommand]
    public void LoginAsDelegate()
    {
        _mainViewModel.NavigateToDashboard("Delegate");
    }

     [RelayCommand]
    public void LoginAsStudent()
    {
        _mainViewModel.NavigateToDashboard("Student");
    }
}
