using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace AddictionsTracker.Dialogs;

public partial class ConfirmationDialog : Window
{

    public ConfirmationDialog() : this(string.Empty) {}

    public ConfirmationDialog(string what)
    {
        InitializeComponent();
        ((TextBlock)textBlock).Text = what;
    }

    public void yesButton_Click(object? sender, RoutedEventArgs args)
    {
        this.Close(true);
    }

    public void noButton_Click(object? sender, RoutedEventArgs args)
    {
        this.Close(false);
    }
}
