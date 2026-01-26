using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System;
using TP203.Models;
using System.Linq;
using System.Threading.Tasks;
using TP203.Services;

namespace TP203.ViewModels;

public partial class StudentDashboardViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainViewModel;
    private readonly User _currentUser;

    [ObservableProperty]
    private string _studentName = "";

    [ObservableProperty]
    private string _classeName = "";

    [ObservableProperty]
    private WeeklyScheduleViewModel _scheduleViewModel;

    public StudentDashboardViewModel(MainWindowViewModel mainViewModel, User user)
    {
        _mainViewModel = mainViewModel;
        _currentUser = user;
        StudentName = user.Login; // Fallback to login if Name is empty
        
        // Init empty
        ScheduleViewModel = new WeeklyScheduleViewModel(new ObservableCollection<DaySchedule>(), false);
        
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            // 1. Identify Student's Level
            var students = await ApiService.GetEtudiantsAsync();
            var studentInfo = students.FirstOrDefault(e => e.FK_Utilisateur == _currentUser.ID_Utilisateur);
            
            int studentNiveauId = studentInfo?.Niveau ?? 0;

            // 2. Fetch Reference Data for mapping
            var seances = await ApiService.GetSeancesAsync();
            var niveaux = (await ApiService.GetNiveauxAsync()).ToDictionary(n => n.Id_Niveau, n => n);
            var salles = (await ApiService.GetSallesAsync()).ToDictionary(s => s.ID_Salle, s => s);
            var ues = (await ApiService.GetUEsAsync()).ToDictionary(u => u.ID_UE, u => u);
            var users = (await ApiService.GetUtilisateursAsync()).ToDictionary(u => u.ID_Utilisateur, u => u);

            if (studentInfo != null) StudentName = $"{studentInfo.Prenom} {studentInfo.Nom}";
            if (niveaux.TryGetValue(studentNiveauId, out var myNiv)) ClasseName = myNiv.Code;

            // 3. Filter and Map
            var classSeances = seances
                .Where(s => s.Niveau_ID == studentNiveauId)
                .ToList();

            foreach (var s in classSeances)
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
                
                var dailySeances = classSeances.Where(s => s.Date.Date == date.Date).OrderBy(s => s.HeureDebut);
                foreach(var s in dailySeances) daySch.Seances.Add(s);

                days.Add(daySch);
            }

            ScheduleViewModel.Days = days;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading student data: {ex.Message}");
        }
    }

    [RelayCommand]
    private void Logout()
    {
        _mainViewModel.Logout();
    }
}
