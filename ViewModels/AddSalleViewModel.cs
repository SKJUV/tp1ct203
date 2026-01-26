using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;
using TP203.Models;
using TP203.Services;

namespace TP203.ViewModels;

public partial class AddSalleViewModel : ViewModelBase
{
    private readonly Action _closeAction;

    [ObservableProperty] private string _nom;
    [ObservableProperty] private string _type = "Amphi";
    [ObservableProperty] private int _capacite = 50;
    [ObservableProperty] private string _errorMessage;
    [ObservableProperty] private bool _isBusy;

    public string[] AvailableTypes { get; } = { "Amphi", "Salle TD", "Labo", "Salle informatique" };

    public AddSalleViewModel(Action closeAction)
    {
        _closeAction = closeAction;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(Nom))
        {
            ErrorMessage = "Le nom est requis";
            return;
        }

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            var salle = new Salle
            {
                Nom = Nom,
                Type = Type,
                Capacite = Capacite
            };

            var success = await ApiService.CreateSalleAsync(salle);
            if (success)
            {
                _closeAction?.Invoke();
            }
            else
            {
                ErrorMessage = "Erreur lors de la création de la salle";
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
