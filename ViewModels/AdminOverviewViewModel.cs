using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System;
using System.Linq;
using TP203.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TP203.ViewModels;

public class DaySchedule
{
    public string DayName { get; set; }
    public DateTime Date { get; set; }
    public ObservableCollection<Seance> Seances { get; set; } = new();

    public DaySchedule(string dayName, DateTime date)
    {
        DayName = dayName;
        Date = date;
    }
}

public partial class AdminOverviewViewModel : AdminSubViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<string> _filieres;

    [ObservableProperty]
    private string _selectedFiliere;

    [ObservableProperty]
    private WeeklyScheduleViewModel _scheduleViewModel;

    private List<Seance> _allSeancesCache; // To store all sessions before filtering
    
    // Lookups for mapping
    private Dictionary<int, Niveau> _niveauLookup = new();
    private Dictionary<int, Salle> _salleLookup = new();
    private Dictionary<int, UniteEnseignement> _ueLookup = new();
    private Dictionary<int, User> _userLookup = new();

    public AdminOverviewViewModel()
    {
        Filieres = new ObservableCollection<string>();
        
        // Init empty first
        ScheduleViewModel = new WeeklyScheduleViewModel(new ObservableCollection<DaySchedule>(), true); // Admin Mode = true

        // Async Load
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        // 1. Fetch Reference Data for mapping
        var niveaux = await Services.ApiService.GetNiveauxAsync();
        _niveauLookup = niveaux.ToDictionary(n => n.Id_Niveau, n => n);
        
        var salles = await Services.ApiService.GetSallesAsync();
        _salleLookup = salles.ToDictionary(s => s.ID_Salle, s => s);
        
        var ues = await Services.ApiService.GetUEsAsync();
        _ueLookup = ues.ToDictionary(u => u.ID_UE, u => u);
        
        var users = await Services.ApiService.GetUtilisateursAsync();
        _userLookup = users.ToDictionary(u => u.ID_Utilisateur, u => u);

        // Update Filieres collection for dropdown
        Filieres.Clear();
        foreach (var n in niveaux)
        {
            if (!Filieres.Contains(n.Code))
                Filieres.Add(n.Code);
        }

        if (Filieres.Count > 0 && string.IsNullOrEmpty(SelectedFiliere))
            SelectedFiliere = Filieres[0];

        // 2. Fetch Seances
        var seances = await Services.ApiService.GetSeancesAsync();
        
        // 3. Map Seances to full objects
        foreach (var s in seances)
        {
            if (_ueLookup.TryGetValue(s.UE_ID, out var ue)) s.UE = ue;
            if (_salleLookup.TryGetValue(s.Salle_ID, out var sl)) s.Salle = sl;
            if (_userLookup.TryGetValue(s.Enseignant_ID, out var t)) s.Enseignant = t;
            if (_niveauLookup.TryGetValue(s.Niveau_ID, out var n)) s.Classe = new Classe { Id_Niveau = n.Id_Niveau, Code = n.Code };
        }

        _allSeancesCache = seances;
        
        // 4. Update UI
        UpdateScheduleFilter();
    }

    partial void OnSelectedFiliereChanged(string value)
    {
        UpdateScheduleFilter();
    }

    [RelayCommand]
    private async Task AddSeance()
    {
        // 1. Create the Window
        var window = new Views.AddSeanceWindow();

        // 2. Create VM and inject Close Action
        var vm = new AddSeanceViewModel(() => 
        {
            window.Close(); 
        });

        // Initialize VM with real data from API
        await vm.LoadDataAsync();

        window.DataContext = vm;

        // 3. Find parent window to use as owner (centering)
        if (Avalonia.Application.Current.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
        {
             await window.ShowDialog(desktop.MainWindow);
        }

        // 4. Refresh Schedule after close
        await LoadDataAsync();
    }

    [RelayCommand]
    private async Task AddNiveau()
    {
        var window = new Views.AddNiveauWindow();
        var vm = new AddNiveauViewModel(() => window.Close());
        window.DataContext = vm;

        if (Avalonia.Application.Current.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
        {
            await window.ShowDialog(desktop.MainWindow);
        }

        await LoadDataAsync();
    }

    private void UpdateScheduleFilter()
    {
        if (_allSeancesCache == null) return;

        // Re-build WeeklySchedule structure
        var newWeek = new ObservableCollection<DaySchedule>();
        DateTime startOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + 1);

        for (int i = 0; i < 6; i++)
        {
            var date = startOfWeek.AddDays(i);
            var daySch = new DaySchedule(date.ToString("dddd", System.Globalization.CultureInfo.CurrentCulture).ToUpper(), date);
            
            // Filter seances for this day AND the selected filiere (Code comparison)
            var seancesForDay = _allSeancesCache
                .Where(s => s.Date.Date == date.Date && (s.Classe?.Code == SelectedFiliere || _niveauLookup.GetValueOrDefault(s.Niveau_ID)?.Code == SelectedFiliere))
                .OrderBy(s => s.HeureDebut)
                .ToList();

            foreach(var s in seancesForDay)
            {
                daySch.Seances.Add(s);
            }

            newWeek.Add(daySch);
        }

        ScheduleViewModel.Days = newWeek;
    }

    private Seance CreateSeance(string code, string prof, string salle, int hDeb, int hFin, SeanceType type, DateTime date, string classeName)
    {
        return new Seance
        {
            UE = new UniteEnseignement { Code = code },
            Enseignant = new User { Name = prof },
            Salle = new Salle { Nom = salle },
            HeureDebut = new TimeSpan(hDeb, 0, 0),
            HeureFin = new TimeSpan(hFin, 0, 0),
            Type = type,
            Date = date,
            Classe = new Classe { Code = classeName }
        };
    }
}
