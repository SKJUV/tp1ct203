using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System;
using TP203.Models;

using System.Linq;
using TP203.Services;

namespace TP203.ViewModels;

public partial class StudentDashboardViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainViewModel;
    
    [ObservableProperty]
    private string _studentName = "Jean Dupont";

    [ObservableProperty]
    private string _classeName = "ICT-L2";

    [ObservableProperty]
    private WeeklyScheduleViewModel _scheduleViewModel;

    public StudentDashboardViewModel(MainWindowViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
        LoadMockData();
    }

    private void LoadMockData()
    {
         var studentClass = "ICT-L2";
         ClasseName = studentClass;

         // Fetch from centralized provider
         var classSeances = DataProvider.AllSeances
             .Where(s => s.Classe.Code == studentClass)
             .OrderBy(s => s.Date).ThenBy(s => s.HeureDebut)
             .ToList();

          var days = new ObservableCollection<DaySchedule>();
          DateTime startOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + 1);

          for (int i = 0; i < 6; i++)
          {
             var date = startOfWeek.AddDays(i);
             var daySch = new DaySchedule(date.ToString("dddd", System.Globalization.CultureInfo.CurrentCulture).ToUpper(), date);
             
             var dailySeances = classSeances.Where(s => s.Date.Date == date.Date);
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
