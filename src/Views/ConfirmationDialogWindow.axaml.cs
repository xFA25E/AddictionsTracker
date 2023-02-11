using System;
using System.Reactive.Linq;
using AddictionsTracker.ViewModels;
using Avalonia.ReactiveUI;
using ReactiveUI;

namespace AddictionsTracker.Views;

public partial class ConfirmationDialogWindow : ReactiveWindow<ConfirmationDialogWindowViewModel>
{
    public ConfirmationDialogWindow()
    {
        InitializeComponent();
        this.WhenActivated(d =>
        {
            var vm = ViewModel!;
            d(Observable.Merge(
                  vm.YesCommand.Select(_ => true),
                  vm.NoCommand.Select(_ => false)
              ).Take(1).Subscribe(r => this.Close(r)));
        });
    }
}
