using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Linq;
using System;
using TP203.Models;
using System.Threading.Tasks;
using TP203.Services;
using System.Collections.Generic;
using TP203.Views;

namespace TP203.ViewModels;

public partial class TeacherDashboardViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainViewModel;
    private readonly User _currentUser;
    
    [ObservableProperty]
    private string _teacherName = "";

    [ObservableProperty]
    private WeeklyScheduleViewModel _scheduleViewModel;

    [ObservableProperty]
    private ViewModelBase _currentSubView;

    [ObservableProperty] private bool _isBusy;
    
    // Data cache
    private List<Etudiant> _allStudents = new();
    private List<Salle> _allSalles = new();

    public TeacherDashboardViewModel(MainWindowViewModel mainViewModel, User user)
    {
        _mainViewModel = mainViewModel;
        _currentUser = user;
        TeacherName = user.Name;
        
        // Init empty
        ScheduleViewModel = new WeeklyScheduleViewModel(new ObservableCollection<DaySchedule>(), false);
        CurrentSubView = new TeacherScheduleOverviewViewModel(ScheduleViewModel);
        
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        IsBusy = true;
        try
        {
            // 1. Fetch data for mapping (Safer approach)
            var seancesTask = ApiService.GetSeancesAsync();
            var niveauxTask = ApiService.GetNiveauxAsync();
            var sallesTask = ApiService.GetSallesAsync();
            var uesTask = ApiService.GetUEsAsync();
            var usersTask = ApiService.GetUtilisateursAsync();

            await Task.WhenAll(seancesTask, niveauxTask, sallesTask, uesTask, usersTask);

            var seances = await seancesTask;
            var niveaux = (await niveauxTask).GroupBy(n => n.Id_Niveau).ToDictionary(g => g.Key, g => g.First());
            var salles = (await sallesTask).GroupBy(s => s.ID_Salle).ToDictionary(g => g.Key, g => g.First());
            var ues = (await uesTask).GroupBy(u => u.ID_UE).ToDictionary(g => g.Key, g => g.First());
            var users = (await usersTask).GroupBy(u => u.ID_Utilisateur).ToDictionary(g => g.Key, g => g.First());

            // 2. Map and Filter for this teacher
            var teacherSeances = seances
                .Where(s => s.Enseignant_ID == _currentUser.ID_Utilisateur)
                .ToList();

            foreach (var s in teacherSeances)
            {
                if (ues.TryGetValue(s.UE_ID, out var ue)) s.UE = ue;
                if (salles.TryGetValue(s.Salle_ID, out var sl)) s.Salle = sl;
                if (users.TryGetValue(s.Enseignant_ID, out var t)) s.Enseignant = t;
                if (niveaux.TryGetValue(s.Niveau_ID, out var n)) s.Classe = new Classe { Id_Niveau = n.Id_Niveau, Code = n.Code };
            }

            var days = new ObservableCollection<DaySchedule>();
            DateTime startOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + 1);

            for (int i = 0; i < 6; i++)
            {
                var date = startOfWeek.AddDays(i);
                var daySch = new DaySchedule(date.ToString("dddd", System.Globalization.CultureInfo.CurrentCulture).ToUpper(), date);
                
                var dailySeances = teacherSeances.Where(s => s.Date.Date == date.Date).OrderBy(s => s.HeureDebut);
                foreach(var s in dailySeances) daySch.Seances.Add(s);

                days.Add(daySch);
            }

            ScheduleViewModel.Days = days;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"LoadData Error: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task OpenAddSeance()
    {
        var vm = new AddSeanceViewModel(() => _ = LoadDataAsync());
        // Pre-select current teacher
        vm.SelectedProf = vm.AvailableProfs.FirstOrDefault(p => p.ID_Utilisateur == _currentUser.ID_Utilisateur);
        
        var win = new AddSeanceWindow { DataContext = vm };
        await win.ShowDialog(TP203.Views.MainWindow.Instance);
    }

    [RelayCommand]
    private async Task Navigate(string target)
    {
        IsBusy = true;
        try
        {
            switch (target)
            {
                case "Schedule":
                    CurrentSubView = new TeacherScheduleOverviewViewModel(ScheduleViewModel);
                    await LoadDataAsync();
                    break;
                case "Students":
                    var allUes = await ApiService.GetUEsAsync();
                    var myUesLevels = allUes.Where(u => u.FK_Enseignant == _currentUser.ID_Utilisateur).Select(u => u.FK_Classe).Distinct().ToList();
                    var students = await ApiService.GetEtudiantsAsync();
                    var myStudents = students.Where(s => myUesLevels.Contains(s.Niveau)).ToList();
                    CurrentSubView = new TeacherStudentsViewModel(new ObservableCollection<Etudiant>(myStudents));
                    break;
                case "Salles":
                    var salles = await ApiService.GetSallesAsync();
                    CurrentSubView = new TeacherSallesViewModel(new ObservableCollection<Salle>(salles));
                    break;
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void Logout()
    {
        _mainViewModel.Logout();
    }
}
