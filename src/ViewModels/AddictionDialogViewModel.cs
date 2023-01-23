using System.Reactive;
using ReactiveUI;
using System.Collections.Generic;
using System.Linq;
using AddictionsTracker.Models;

namespace AddictionsTracker.ViewModels;

class AddictionDialogViewModel : ViewModelBase
{
    string title;

    ISet<string> TitlesToIgnore { get; }

    public AddictionDialogViewModel(ISet<string> titlesToIgnore)
        : this(titlesToIgnore, string.Empty)
    {
    }

    public AddictionDialogViewModel(
        ISet<string> titlesToIgnore,
        string initialTitle
    )
    {
        title = initialTitle;
        TitlesToIgnore = titlesToIgnore;

        var okEnabled = this.WhenAnyValue(
            x => x.Title,
            x => !(string.IsNullOrWhiteSpace(x) || TitlesToIgnore.Contains(x))
        );

        Ok = ReactiveCommand.Create(() => new Addiction(Title), okEnabled);
        Cancel = ReactiveCommand.Create(() => { });

    }

    public string Title
    {
        get => title;
        set => this.RaiseAndSetIfChanged(ref title, value);
    }

    public ReactiveCommand<Unit, Addiction> Ok { get; }
    public ReactiveCommand<Unit, Unit> Cancel { get; }
}
