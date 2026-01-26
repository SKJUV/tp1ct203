using Avalonia.Controls;

namespace TP203.Views;

public partial class MainWindow : Window
{
    public static MainWindow Instance { get; private set; }

    public MainWindow()
    {
        InitializeComponent();
        Instance = this;
    }
}