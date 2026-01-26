using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TP203.Models;
using TP203.Services;

namespace TP203.ViewModels;

public partial class AddFiliereViewModel : ViewModelBase
{
    private readonly Action _closeAction;

    [ObservableProperty] private string _intitule;
    [ObservableProperty] private string _code;
    [ObservableProperty] private Departement _selectedDepartement;
    [ObservableProperty] private ObservableCollection<Departement> _departements = new();
    [ObservableProperty] private string _errorMessage;
    [ObservableProperty] private bool _isBusy;

    public AddFiliereViewModel(Action closeAction)
    {
        _closeAction = closeAction;
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        var depts = await ApiService.GetDepartementsAsync();
        Departements = new ObservableCollection<Departement>(depts);
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(Intitule) || string.IsNullOrWhiteSpace(Code) || SelectedDepartement == null)
        {
            ErrorMessage = "Veuillez remplir tous les champs";
            return;
        }

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            var filiere = new Filiere 
            { 
                Intitule = Intitule, 
                Code = Code, 
                Departement = SelectedDepartement.ID_Departement 
            };
            bool success = await ApiService.CreateFiliereAsync(filiere);
            if (success) _closeAction?.Invoke();
            else ErrorMessage = "Erreur lors de la création de la filière";
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
