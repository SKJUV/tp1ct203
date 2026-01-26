using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TP203.Models;
using TP203.Services;

namespace TP203.ViewModels;

public partial class RegisterViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainViewModel;

    [ObservableProperty] private string _login;
    [ObservableProperty] private string _password;
    [ObservableProperty] private UserType _selectedType = UserType.Etudiant;
    [ObservableProperty] private string _errorMessage;
    [ObservableProperty] private bool _isBusy;

    public ObservableCollection<UserType> AvailableTypes { get; } = new()
    {
        UserType.Enseignant,
        UserType.Etudiant,
        UserType.Delegue,
        UserType.Admin
    };

    public RegisterViewModel(MainWindowViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
    }

    [RelayCommand]
    private async Task RegisterAsync()
    {
        if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Veuillez remplir tous les champs";
            return;
        }

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            var user = new User
            {
                Login = Login,
                Type = (int)SelectedType,
                Actif = true
            };

            bool success = await ApiService.CreateUtilisateurAsync(user, Password);
            if (success)
            {
                // After successful registration, navigate back to login or auto-login
                _mainViewModel.Logout(); 
            }
            else
            {
                ErrorMessage = "Erreur lors de la création du compte";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Erreur: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void BackToLogin()
    {
        _mainViewModel.Logout();
    }
}
