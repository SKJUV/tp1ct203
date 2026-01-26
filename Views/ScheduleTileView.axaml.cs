using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using TP203.Models;

namespace TP203.Views;

public partial class ScheduleTileView : UserControl
{
    public ScheduleTileView()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(System.EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is Seance seance)
        {
            var indicator = this.FindControl<Avalonia.Controls.Shapes.Rectangle>("TypeIndicator");
            if (indicator != null)
            {
                indicator.Fill = seance.Type switch
                {
                    SeanceType.CM => (Avalonia.Media.IBrush)App.Current!.Resources["CMBrush"]!,
                    SeanceType.TP => (Avalonia.Media.IBrush)App.Current!.Resources["TPBrush"]!,
                    SeanceType.CC => (Avalonia.Media.IBrush)App.Current!.Resources["CCBrush"]!,
                    _ => (Avalonia.Media.IBrush)App.Current!.Resources["CMBrush"]!
                };
            }
        }
    }
}
