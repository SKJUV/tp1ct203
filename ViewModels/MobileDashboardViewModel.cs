using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using TP203.Models;

namespace TP203.ViewModels;

public partial class StudentPresence : ObservableObject
{
    [ObservableProperty] private string _name;
    [ObservableProperty] private bool _isPresent;
    [ObservableProperty] private bool _isAtRisk; // Absences > 30%

    public StudentPresence(string name, bool present = true, bool atRisk = false)
    {
        _name = name;
        _isPresent = present;
        _isAtRisk = atRisk;
    }
}

public partial class MobileDashboardViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainViewModel;
    
    [ObservableProperty] private string _role;
    [ObservableProperty] private ObservableCollection<StudentPresence> _students;
    [ObservableProperty] private Seance _currentSeance;
    [ObservableProperty] private bool _isCertified;

    public MobileDashboardViewModel(MainWindowViewModel mainViewModel, string role)
    {
        _mainViewModel = mainViewModel;
        _role = role;

        // Mock Data for Delegate ICT-L2
        _students = new ObservableCollection<StudentPresence>
        {
            new StudentPresence("Amina B.", true),
            new StudentPresence("Jean Dupont", true),
            new StudentPresence("Sophie Germain", false, true), // At risk
            new StudentPresence("Isaac Newton", true),
            new StudentPresence("Marie Curie", true),
            new StudentPresence("Albert Einstein", true)
        };

        _currentSeance = new Seance 
        { 
            UE = new UniteEnseignement { Code = "MAT201", Nom = "Statistiques" },
            Enseignant = new User { Name = "Pr. Fofana" },
            Salle = new Salle { Nom = "Salle A" },
            HeureDebut = new System.TimeSpan(8,0,0),
            HeureFin = new System.TimeSpan(10,0,0)
        };
    }

    [RelayCommand]
    private void Certify()
    {
        IsCertified = true;
    }

    [RelayCommand]
    private void Logout()
    {
        _mainViewModel.Logout();
    }
}
