using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using TP203.Models;

namespace TP203.ViewModels;

public abstract class TeacherSubViewModelBase : ViewModelBase
{
}

public partial class TeacherStudentsViewModel : TeacherSubViewModelBase
{
    [ObservableProperty] private ObservableCollection<Etudiant> _students = new();
    [ObservableProperty] private bool _isBusy;

    public TeacherStudentsViewModel(ObservableCollection<Etudiant> students)
    {
        Students = students;
    }
}

public partial class TeacherSallesViewModel : TeacherSubViewModelBase
{
    [ObservableProperty] private ObservableCollection<Salle> _salles = new();
    [ObservableProperty] private bool _isBusy;

    public TeacherSallesViewModel(ObservableCollection<Salle> salles)
    {
        Salles = salles;
    }
}

public partial class TeacherScheduleOverviewViewModel : TeacherSubViewModelBase
{
    [ObservableProperty] private WeeklyScheduleViewModel _schedule;

    public TeacherScheduleOverviewViewModel(WeeklyScheduleViewModel schedule)
    {
        Schedule = schedule;
    }
}
