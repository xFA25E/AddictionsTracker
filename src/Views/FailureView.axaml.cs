using Avalonia.Controls;
using Avalonia.Interactivity;

namespace AddictionsTracker.Views;

public partial class FailureView : UserControl
{
    public FailureView()
    {
        InitializeComponent();
    }

    public void OpenContextMenu(object? sender, RoutedEventArgs _)
    {
        if (sender is Control control) control.ContextMenu?.Open();
    }
}
