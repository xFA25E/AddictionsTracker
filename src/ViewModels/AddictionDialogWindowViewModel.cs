using System;
using System.Collections.Generic;
using System.Reactive;
using ReactiveUI;

namespace AddictionsTracker.ViewModels;

public class AddictionDialogWindowViewModel : ViewModelBase
{
    HashSet<string> prohibitedTitles;

    public AddictionDialogWindowViewModel(
        string initialTitle,
        IEnumerable<string> prohibitedTitles
    )
    {
        this.prohibitedTitles = new(prohibitedTitles);
        title = initialTitle;

        OkCommand = ReactiveCommand.Create(
            () => Title,
            this.WhenAnyValue(
                x => x.Title,
                x => !(x.Equals("") || this.prohibitedTitles.Contains(x))
            )
        );
        CancelCommand = ReactiveCommand.Create(() => { });
    }

    string title = "";
    public string Title
    {
        get => title;
        set => this.RaiseAndSetIfChanged(ref title, value);
    }

    public ReactiveCommand<Unit, string> OkCommand { get; }
    public ReactiveCommand<Unit, Unit> CancelCommand { get; }
}
