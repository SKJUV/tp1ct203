using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TP203.Models;
using TP203.Services;

namespace TP203.ViewModels;

public partial class AddNiveauViewModel : ViewModelBase
{
    private readonly Action _closeAction;

    [ObservableProperty] private string _code;
    [ObservableProperty] private string _anneeAcademique = "2025-2026";
    [ObservableProperty] private int _semestre = 1;
    [ObservableProperty] private int _effectif = 50;
    [ObservableProperty] private Filiere _selectedFiliere;
    
    [ObservableProperty] private ObservableCollection<Filiere> _availableFilieres = new();
    
    [ObservableProperty] private string _errorMessage;
    [ObservableProperty] private bool _isBusy;

    public AddNiveauViewModel(Action closeAction)
    {
        _closeAction = closeAction;
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            // Usually you'd fetch filieres from API
            // For now let's mock or use API if exists
            // Selected Swagger shows GET /api/Filiere
            var response = await ApiService.GetFilieresAsync();
            AvailableFilieres = new ObservableCollection<Filiere>(response);
        }
        catch { }
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(Code) || SelectedFiliere == null)
        {
            ErrorMessage = "Veuillez remplir les champs obligatoires";
            return;
        }

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            var niveau = new Niveau
            {
                Code = Code,
                Annee_Academique = AnneeAcademique,
                Semestre = Semestre,
                Effectif = Effectif,
                Filiere = SelectedFiliere.ID_Filiere
            };

            var success = await ApiService.CreateNiveauAsync(niveau);
            if (success)
            {
                _closeAction?.Invoke();
            }
            else
            {
                ErrorMessage = "Erreur lors de la création du niveau";
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
    private void Cancel() => _closeAction?.Invoke();
}
