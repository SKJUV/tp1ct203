using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using TP203.Models;

namespace TP203.ViewModels;

public partial class WeeklyScheduleViewModel : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<DaySchedule> _days;

    [ObservableProperty]
    private bool _isAdminMode;

    public WeeklyScheduleViewModel(ObservableCollection<DaySchedule> days, bool isAdminMode = false)
    {
        _days = days;
        _isAdminMode = isAdminMode;
    }
}
