using System;
using System.Threading.Tasks;
using AddictionsTracker.ViewModels;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using ReactiveUI;

namespace AddictionsTracker.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    public MainWindow()
    {
        InitializeComponent();
        this.WhenActivated(d =>
        {
            d(ViewModel!.ShowAddictionDialog.RegisterHandler(DoShowAddictionDialogAsync));
            d(ViewModel!.ShowConfirmationDialog.RegisterHandler(DoShowConfirmationDialogAsync));
            d(ViewModel!.ShowFailureDialog.RegisterHandler(DoShowFailureDialogAsync));
        });
    }

    private async Task DoShowAddictionDialogAsync(InteractionContext<AddictionDialogWindowViewModel, string?> interaction)
    {
        var dialog = new AddictionDialogWindow();
        dialog.DataContext = interaction.Input;

        var result = await dialog.ShowDialog<string?>(this);
        interaction.SetOutput(result);
    }

    private async Task DoShowConfirmationDialogAsync(InteractionContext<ConfirmationDialogWindowViewModel, bool> interaction)
    {
        var dialog = new ConfirmationDialogWindow();
        dialog.DataContext = interaction.Input;

        var result = await dialog.ShowDialog<bool>(this);
        interaction.SetOutput(result);
    }

    private async Task DoShowFailureDialogAsync(InteractionContext<FailureDialogWindowViewModel, (DateOnly, string)?> interaction)
    {
        var dialog = new FailureDialogWindow();
        dialog.DataContext = interaction.Input;

        var result = await dialog.ShowDialog<(DateOnly, string)?>(this);
        interaction.SetOutput(result);
    }

    public void OpenContextMenu(object? sender, RoutedEventArgs _)
    {
        if (sender is Control control) control.ContextMenu?.Open();
    }

    public void OnPointerWheelChanged(object? sender, PointerWheelEventArgs a)
    {
        if (a.KeyModifiers == KeyModifiers.Control
            && DataContext is MainWindowViewModel mwVM)
        {
            System.Console.WriteLine("Fuck you!");
            mwVM.DayWidth += (int)a.Delta.Y;
            System.Console.WriteLine($"{mwVM.DayWidth}");
        }
    }
}
