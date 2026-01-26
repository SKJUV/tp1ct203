using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using TP203.Services;

namespace TP203.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainViewModel;

    [ObservableProperty]
    private string _email = string.Empty; // Used as Login

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public LoginViewModel(MainWindowViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        ErrorMessage = string.Empty;
        IsBusy = true;

        try
        {
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
            {
                ErrorMessage = "Veuillez remplir tous les champs";
                return;
            }

            // Real API Call
            var (user, error) = await ApiService.LoginAsync(Email, Password);

            if (user != null)
            {
                string roleName = user.Role.ToString();
                
                // Fallback / Patch for Type 0 or unknown
                if (user.Type == 0 || roleName == "0") roleName = "Admin";

                _mainViewModel.NavigateToDashboard(roleName);
            }
            else
            {
                ErrorMessage = error ?? "Identifiants invalides";
            }
        }
        catch (System.Exception ex)
        {
            ErrorMessage = "Erreur de connexion serveur";
            System.Diagnostics.Debug.WriteLine(ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }
}
