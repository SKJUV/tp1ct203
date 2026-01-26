using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TP203.Models;
using TP203.Services;

namespace TP203.ViewModels;

public abstract class AdminSubViewModelBase : ViewModelBase
{
}



public partial class SalleViewModel : ObservableObject
{
    [ObservableProperty] private string _nom;
    [ObservableProperty] private int _capacite;
    [ObservableProperty] private string _type;
    [ObservableProperty] private string _equipements; // Ex: "Videoprojecteur, Clim"
    [ObservableProperty] private int _tauxOccupation; // Pour la démo (ex: 75%)

    public SalleViewModel(Salle salle)
    {
        _nom = salle.Nom;
        _capacite = salle.Capacite;
        _type = salle.Type;
        
        // Mock logic for demo visual appeal
        _equipements = _type == "Labo" ? "PCs, Réseau, Projecteur" : "Sonorisation, Estrade";
        _tauxOccupation = _type == "Amphi" ? 85 : 40;
    }
}

public partial class AdminSallesViewModel : AdminSubViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<SalleViewModel> _salesDisplay = new();

    public AdminSallesViewModel()
    {
        _ = LoadDataAsync();
    }

    [RelayCommand]
    private async Task AddSalle()
    {
        var window = new Views.AddSalleWindow();
        var vm = new AddSalleViewModel(() => window.Close());
        window.DataContext = vm;

        if (Avalonia.Application.Current.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
        {
            await window.ShowDialog(desktop.MainWindow);
        }

        await LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        var salles = await ApiService.GetSallesAsync();
        var vms = new ObservableCollection<SalleViewModel>();
        foreach (var s in salles)
        {
            vms.Add(new SalleViewModel(s));
        }
        SalesDisplay = vms;
    }
}
