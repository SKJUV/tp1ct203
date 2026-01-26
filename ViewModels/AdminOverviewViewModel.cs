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
        // 1. Fetch Filieres (Niveaux)
        var niveaux = await Services.ApiService.GetNiveauxAsync();
        foreach (var n in niveaux)
        {
            if (!Filieres.Contains(n.Code))
                Filieres.Add(n.Code);
        }

        if (Filieres.Count > 0 && string.IsNullOrEmpty(SelectedFiliere))
            SelectedFiliere = Filieres[0];

        // 2. Fetch Seances
        _allSeancesCache = await Services.ApiService.GetSeancesAsync();
        
        // 3. Update UI
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
            // Note: In real scenarios we might use Level ID
            var seancesForDay = _allSeancesCache
                .Where(s => s.Date.Date == date.Date && (s.Niveau_ID.ToString() == SelectedFiliere || s.Classe?.Code == SelectedFiliere))
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
