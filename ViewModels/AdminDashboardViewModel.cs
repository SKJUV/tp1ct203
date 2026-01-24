using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace TP203.ViewModels;

public partial class AdminDashboardViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainViewModel;

    [ObservableProperty]
    private string _activeSection = "Planning"; // Planning, Resources, Audit

    [ObservableProperty]
    private string _notificationCount = "3";

    public ObservableCollection<string> ConflictLogs { get; } = new();

    public AdminDashboardViewModel(MainWindowViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
        
        // Mock Logs
        ConflictLogs.Add("Conflict: Room A occupied by ICT-L2");
        ConflictLogs.Add("Warning: Dr. Kemgou prefers not Tue 8-10");
        ConflictLogs.Add("Success: MAT201 scheduled successfully");
    }

    [RelayCommand]
    public void Logout()
    {
        _mainViewModel.Logout();
    }
}
