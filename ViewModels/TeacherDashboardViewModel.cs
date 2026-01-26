using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Linq;
using System;
using TP203.Models;

namespace TP203.ViewModels;

public partial class TeacherDashboardViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainViewModel;
    
    [ObservableProperty]
    private string _teacherName = "Pr. Fofana";

    [ObservableProperty]
    private WeeklyScheduleViewModel _scheduleViewModel;

    public TeacherDashboardViewModel(MainWindowViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
        LoadMockData();
    }

    private void LoadMockData()
    {
        // Get all sessions for this specific teacher
        // Note: In DataProvider we used "Pr. Fofana" for some courses
        
        var teacherSeances = Services.DataProvider.AllSeances
            .Where(s => s.Enseignant.Name == TeacherName)
            .OrderBy(s => s.Date).ThenBy(s => s.HeureDebut)
            .ToList();

         var days = new ObservableCollection<DaySchedule>();
         DateTime startOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + 1);

         for (int i = 0; i < 6; i++)
         {
            var date = startOfWeek.AddDays(i);
            var daySch = new DaySchedule(date.ToString("dddd", System.Globalization.CultureInfo.CurrentCulture).ToUpper(), date);
            
            var dailySeances = teacherSeances.Where(s => s.Date.Date == date.Date);
            foreach(var s in dailySeances) daySch.Seances.Add(s);

            days.Add(daySch);
         }

        ScheduleViewModel = new WeeklyScheduleViewModel(days, false);
    }

    [RelayCommand]
    private void Logout()
    {
        _mainViewModel.Logout();
    }
}
