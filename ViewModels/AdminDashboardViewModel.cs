using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using TP203.Models;

namespace TP203.ViewModels;

public partial class AdminDashboardViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainViewModel;

    [ObservableProperty]
    private AdminSubViewModelBase _currentSubView;

    [ObservableProperty]
    private ObservableCollection<UniteEnseignement> _unitesEnseignement;

    [ObservableProperty]
    private ObservableCollection<Seance> _recentSeances;

    public AdminDashboardViewModel(MainWindowViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
        
        // Initialize Data (Mock)
        InitializeMockData();

        // Default view
        _currentSubView = new AdminOverviewViewModel();
    }

    private void InitializeMockData()
    {
        var drKemgou = new User { Name = "Dr. Kemgou", Role = UserType.Enseignant };
        var prFofana = new User { Name = "Pr. Fofana", Role = UserType.Enseignant };
        
        var ict203 = new UniteEnseignement { Code = "ICT203", Nom = "Génie Logiciel", VolumeHoraireTotal = 45, VolumeHoraireEffectue = 32 };
        var mat201 = new UniteEnseignement { Code = "MAT201", Nom = "Statistiques", VolumeHoraireTotal = 30, VolumeHoraireEffectue = 12 };

        _unitesEnseignement = new ObservableCollection<UniteEnseignement> { ict203, mat201 };
        _recentSeances = new ObservableCollection<Seance>
        {
            new Seance { Date = DateTime.Now, HeureDebut = new TimeSpan(8,0,0), HeureFin = new TimeSpan(10,0,0), UE = ict203, Enseignant = drKemgou, Salle = new Salle { Nom = "Salle A" } },
            new Seance { Date = DateTime.Now, HeureDebut = new TimeSpan(10,0,0), HeureFin = new TimeSpan(12,0,0), UE = mat201, Enseignant = prFofana, Salle = new Salle { Nom = "Salle A" } }
        };
    }

    [RelayCommand]
    private void Navigate(string target)
    {
        CurrentSubView = target switch
        {
            "Salles" => new AdminSallesViewModel(),
            "Suivi" => new AdminSuiviViewModel(),
            _ => new AdminOverviewViewModel()
        };
    }

    [RelayCommand]
    private void Logout()
    {
        _mainViewModel.Logout();
    }
}
