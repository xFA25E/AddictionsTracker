using System;
using System.Reactive.Linq;
using AddictionsTracker.ViewModels;
using Avalonia.ReactiveUI;
using ReactiveUI;

namespace AddictionsTracker.Views;

public partial class FailureDialogWindow : ReactiveWindow<FailureDialogWindowViewModel>
{
    public FailureDialogWindow()
    {
        InitializeComponent();
        this.WhenActivated(d =>
        {
            var vm = ViewModel!;
            d(Observable.Merge(
                  vm.OkCommand.Select(f => ((DateOnly, string)?)f),
                  vm.CancelCommand.Select(_ => ((DateOnly, string)?)null)
              ).Take(1).Subscribe(d => this.Close(d)));
        });
    }
}
