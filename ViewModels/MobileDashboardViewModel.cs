using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using TP203.Models;
using System.Linq;
using TP203.Services;
using System.Threading.Tasks;

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
    private readonly User _currentUser;
    [ObservableProperty] private string _role;
    [ObservableProperty] private ObservableCollection<StudentPresence> _students = new();
    [ObservableProperty] private Seance _currentSeance;
    [ObservableProperty] private bool _isCertified;

    public MobileDashboardViewModel(MainWindowViewModel mainViewModel, User user)
    {
        _mainViewModel = mainViewModel;
        _currentUser = user;
        _role = user.Role.ToString();

        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        var allStudents = await ApiService.GetEtudiantsAsync();
        
        // Filter students by the same level as the delegate
        var me = allStudents.FirstOrDefault(e => e.FK_Utilisateur == _currentUser.ID_Utilisateur);
        if (me != null)
        {
            var myClassmates = allStudents.Where(e => e.Niveau == me.Niveau);
            foreach (var s in myClassmates)
            {
                Students.Add(new StudentPresence($"{s.Prenom} {s.Nom}", true));
            }
        }
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
