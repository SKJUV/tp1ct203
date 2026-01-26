using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TP203.Models;
using TP203.Services;
using System.Linq;
using System;

namespace TP203.ViewModels;

public partial class AdminStructureViewModel : AdminSubViewModelBase
{
    [ObservableProperty] private ObservableCollection<Departement> _departements = new();
    [ObservableProperty] private ObservableCollection<Filiere> _filieres = new();
    [ObservableProperty] private ObservableCollection<Niveau> _niveaux = new();
    [ObservableProperty] private bool _isBusy;

    public AdminStructureViewModel()
    {
        _ = LoadDataAsync();
    }

    public async Task LoadDataAsync()
    {
        IsBusy = true;
        try
        {
            var deptsTask = ApiService.GetDepartementsAsync();
            var filieresTask = ApiService.GetFilieresAsync();
            var niveauxTask = ApiService.GetNiveauxAsync();

            await Task.WhenAll(deptsTask, filieresTask, niveauxTask);

            Departements = new ObservableCollection<Departement>(await deptsTask);
            Filieres = new ObservableCollection<Filiere>(await filieresTask);
            Niveaux = new ObservableCollection<Niveau>(await niveauxTask);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task AddDepartement()
    {
        var window = new Views.AddDepartmentWindow();
        var vm = new AddDepartmentViewModel(() => window.Close());
        window.DataContext = vm;
        
        if (Avalonia.Application.Current.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
        {
            await window.ShowDialog(desktop.MainWindow);
        }
        await LoadDataAsync();
    }

    [RelayCommand]
    private async Task AddFiliere()
    {
        var window = new Views.AddFiliereWindow();
        var vm = new AddFiliereViewModel(() => window.Close());
        window.DataContext = vm;

        if (Avalonia.Application.Current.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
        {
            await window.ShowDialog(desktop.MainWindow);
        }
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

    [RelayCommand]
    private async Task DeleteDepartement(Departement dept)
    {
        if (dept == null) return;
        if (await ApiService.DeleteDepartementAsync(dept.ID_Departement))
            await LoadDataAsync();
    }

    [RelayCommand]
    private async Task DeleteFiliere(Filiere filiere)
    {
        if (filiere == null) return;
        if (await ApiService.DeleteFiliereAsync(filiere.ID_Filiere))
            await LoadDataAsync();
    }

    [RelayCommand]
    private async Task DeleteNiveau(Niveau niveau)
    {
        if (niveau == null) return;
        if (await ApiService.DeleteNiveauAsync(niveau.Id_Niveau))
            await LoadDataAsync();
    }
}
