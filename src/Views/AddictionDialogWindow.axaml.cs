using System;
using System.Reactive.Linq;
using AddictionsTracker.ViewModels;
using Avalonia.ReactiveUI;
using ReactiveUI;

namespace AddictionsTracker.Views;

public partial class AddictionDialogWindow : ReactiveWindow<AddictionDialogWindowViewModel>
{
    public AddictionDialogWindow()
    {
        InitializeComponent();
        this.WhenActivated(d =>
        {
            var vm = ViewModel!;
            d(Observable.Merge(
                  vm.OkCommand,
                  vm.CancelCommand.Select(_ => (string?)null)
              ).Take(1).Subscribe(Close));
        });
    }
}
