using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;
using TP203.Models;
using TP203.Services;

namespace TP203.ViewModels;

public partial class AddDepartmentViewModel : ViewModelBase
{
    private readonly Action _closeAction;

    [ObservableProperty] private string _nom;
    [ObservableProperty] private string _code;
    [ObservableProperty] private string _errorMessage;
    [ObservableProperty] private bool _isBusy;

    public AddDepartmentViewModel(Action closeAction)
    {
        _closeAction = closeAction;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(Nom) || string.IsNullOrWhiteSpace(Code))
        {
            ErrorMessage = "Veuillez remplir tous les champs";
            return;
        }

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            var dept = new Departement { Nom = Nom, Code = Code };
            bool success = await ApiService.CreateDepartementAsync(dept);
            if (success) _closeAction?.Invoke();
            else ErrorMessage = "Erreur lors de la création du département";
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void Cancel() => _closeAction?.Invoke();
}
