using System.Reactive;
using ReactiveUI;

namespace AddictionsTracker.ViewModels;

class DeletionDialogViewModel : ViewModelBase
{
    string Text { get; }

    public DeletionDialogViewModel(string text)
    {
        Text = text;
        Yes = ReactiveCommand.Create(() => { });
        No = ReactiveCommand.Create(() => { });

    }

    public ReactiveCommand<Unit, Unit> Yes { get; }
    public ReactiveCommand<Unit, Unit> No { get; }
}
