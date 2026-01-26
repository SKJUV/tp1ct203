using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TP203.Models;
using TP203.Services;

namespace TP203.ViewModels;

public partial class AdminSuiviViewModel : AdminSubViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<UniteEnseignement> _ues = new();

    public AdminSuiviViewModel()
    {
        _ = LoadDataAsync();
    }

    [RelayCommand]
    private async Task AddUE()
    {
        var window = new Views.AddUEWindow();
        var vm = new AddUEViewModel(() => window.Close());
        window.DataContext = vm;

        if (Avalonia.Application.Current.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
        {
            await window.ShowDialog(desktop.MainWindow);
        }

        await LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        var apiUEs = await ApiService.GetUEsAsync();
        
        // Compute mockup progression since API might not return it yet
        foreach(var ue in apiUEs)
        {
            ue.VolumeHoraireEffectue = (ue.Volume_Horaire_Total * 0.6); // 60% fixed for demo
        }

        Ues = new ObservableCollection<UniteEnseignement>(apiUEs);
    }
}
