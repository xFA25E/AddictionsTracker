using System;
using System.Collections.Generic;
using System.Reactive;
using ReactiveUI;

namespace AddictionsTracker.ViewModels;

public class FailureDialogWindowViewModel : ViewModelBase
{
    HashSet<DateOnly> prohibitedDates;

    public FailureDialogWindowViewModel(
        DateOnly initialDate,
        string initialNote,
        IEnumerable<DateOnly> prohibitedDates
    )
    {
        this.prohibitedDates = new(prohibitedDates);
        Date = initialDate.ToDateTimeOffset();
        Note = initialNote;

        OkCommand = ReactiveCommand.Create(
            () => (Date.ToDateOnly(), Note),
            this.WhenAnyValue(
                x => x.Date,
                x => DateTime.Now.ToDateOnly() >= x.ToDateOnly()
                && !this.prohibitedDates.Contains(x.ToDateOnly())
            )
        );
        CancelCommand = ReactiveCommand.Create(() => { });
    }

    string note = "";
    public string Note
    {
        get => note;
        set => this.RaiseAndSetIfChanged(ref note, value);
    }

    DateTimeOffset date = DateTime.Now.ToDateOnly().ToDateTimeOffset();
    public DateTimeOffset Date
    {
        get => date;
        set => this.RaiseAndSetIfChanged(ref date, value);
    }

    public ReactiveCommand<Unit, (DateOnly, string)> OkCommand { get; }
    public ReactiveCommand<Unit, Unit> CancelCommand { get; }
}
