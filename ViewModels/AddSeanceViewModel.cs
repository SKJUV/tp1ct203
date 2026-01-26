using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TP203.Models;
using TP203.Services;

namespace TP203.ViewModels;

public partial class AddSeanceViewModel : ViewModelBase
{
    private Action _closeAction;

    // Observable Collections for the UI
    [ObservableProperty] private ObservableCollection<UniteEnseignement> _availableUEs = new();
    [ObservableProperty] private ObservableCollection<Salle> _availableSalles = new();
    [ObservableProperty] private ObservableCollection<User> _availableProfs = new(); // Mocked users for now
    [ObservableProperty] private ObservableCollection<Niveau> _availableFilieres = new();
    [ObservableProperty] private ObservableCollection<string> _availableTypes = new() { "CM", "TP", "TD", "EXAM" };

    // Selection
    [ObservableProperty] private UniteEnseignement _selectedUE;
    [ObservableProperty] private Salle _selectedSalle;
    [ObservableProperty] private User _selectedProf;
    [ObservableProperty] private Niveau _selectedFiliere;
    [ObservableProperty] private string _selectedType = "CM";
    
    [ObservableProperty] private DateTimeOffset _selectedDate = DateTimeOffset.Now;
    [ObservableProperty] private TimeSpan _startTime = new TimeSpan(8, 0, 0);
    [ObservableProperty] private TimeSpan _endTime = new TimeSpan(10, 0, 0);

    [ObservableProperty] private string _errorMessage;
    [ObservableProperty] private bool _isBusy;

    public AddSeanceViewModel(Action closeAction)
    {
        _closeAction = closeAction;
    }

    public async Task LoadDataAsync()
    {
        try 
        {
            var ues = await ApiService.GetUEsAsync();
            AvailableUEs = new ObservableCollection<UniteEnseignement>(ues);

            var salles = await ApiService.GetSallesAsync();
            AvailableSalles = new ObservableCollection<Salle>(salles);

            var niveaux = await ApiService.GetNiveauxAsync();
            AvailableFilieres = new ObservableCollection<Niveau>(niveaux);

            // Mocked Professors (as they might not be in a special teacher list endpoint)
            AvailableProfs = new ObservableCollection<User> {
                new User { ID_Utilisateur = 1, Name = "Dr. Kemgou" },
                new User { ID_Utilisateur = 2, Name = "Pr. Fofana" }
            };
        }
        catch { }
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (SelectedUE == null || SelectedFiliere == null || SelectedSalle == null)
        {
            ErrorMessage = "Veuillez remplir tous les champs obligatoires";
            return;
        }

        ErrorMessage = null;
        IsBusy = true;

        try 
        {
            var newSeance = new Seance
            {
                UE_ID = SelectedUE.ID_UE,
                Salle_ID = SelectedSalle.ID_Salle,
                Enseignant_ID = SelectedProf?.ID_Utilisateur ?? 1,
                Niveau_ID = SelectedFiliere.Id_Niveau,
                Statut = "PLANIFIEE"
            };
            
            // Use the alias properties to set the internal Date_Heure fields
            newSeance.Date = SelectedDate.Date;
            newSeance.HeureDebut = StartTime;
            newSeance.HeureFin = EndTime;

            // API Call
            bool success = await ApiService.CreateSeanceAsync(newSeance);

            if (success)
            {
                _closeAction?.Invoke();
            }
            else
            {
                ErrorMessage = "Erreur lors de l'enregistrement sur le serveur";
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
    private void Cancel()
    {
        _closeAction?.Invoke();
    }
}
