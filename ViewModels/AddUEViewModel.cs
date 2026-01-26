using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TP203.Models;
using TP203.Services;

namespace TP203.ViewModels;

public partial class AddUEViewModel : ViewModelBase
{
    private readonly Action _closeAction;

    [ObservableProperty] private string _codeUE;
    [ObservableProperty] private string _intitule;
    [ObservableProperty] private int _volumeHoraireTotal = 45;
    [ObservableProperty] private User _selectedEnseignant;
    [ObservableProperty] private Niveau _selectedNiveau;
    
    [ObservableProperty] private ObservableCollection<User> _availableEnseignants = new();
    [ObservableProperty] private ObservableCollection<Niveau> _availableNiveaux = new();
    
    [ObservableProperty] private string _errorMessage;
    [ObservableProperty] private bool _isBusy;

    public AddUEViewModel(Action closeAction)
    {
        _closeAction = closeAction;
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            var niveaux = await ApiService.GetNiveauxAsync();
            AvailableNiveaux = new ObservableCollection<Niveau>(niveaux);

            // Mocked Professors - in a real app, you might have a GetUsersAsync(role=Enseignant)
            AvailableEnseignants = new ObservableCollection<User> {
                new User { ID_Utilisateur = 1, Name = "Dr. Kemgou" },
                new User { ID_Utilisateur = 2, Name = "Pr. Fofana" },
                new User { ID_Utilisateur = 3, Name = "Mme. Smith" }
            };
        }
        catch { }
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(CodeUE) || string.IsNullOrWhiteSpace(Intitule) || SelectedNiveau == null)
        {
            ErrorMessage = "Veuillez remplir les champs obligatoires";
            return;
        }

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            var ue = new UniteEnseignement
            {
                Code_UE = CodeUE,
                Intitule = Intitule,
                Volume_Horaire_Total = VolumeHoraireTotal,
                FK_Enseignant = SelectedEnseignant?.ID_Utilisateur ?? 1,
                FK_Classe = SelectedNiveau.Id_Niveau
            };

            var success = await ApiService.CreateUniteEnseignementAsync(ue);
            if (success)
            {
                _closeAction?.Invoke();
            }
            else
            {
                ErrorMessage = "Erreur lors de la création de l'UE";
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
