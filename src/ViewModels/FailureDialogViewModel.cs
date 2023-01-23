using System.Reactive;
using ReactiveUI;
using System.Collections.Generic;
using System.Linq;
using AddictionsTracker.Models;
using System;
using Avalonia.Controls;

namespace AddictionsTracker.ViewModels;

class FailureDialogViewModel : ViewModelBase
{
    ISet<DateTime> DatetimesToIgnore { get; }

    DatePicker DatePicker { get; } = new();
    TimePicker TimePicker { get; } = new();

    DateTime failureDateTime;
    DateTime FailureDateTime
    {
        get => failureDateTime;
        set => this.RaiseAndSetIfChanged(ref failureDateTime, value);

    }

    string note;
    string Note
    {
        get => note;
        set => this.RaiseAndSetIfChanged(ref note, value);
    }

    public FailureDialogViewModel(
        ISet<DateTime> datetimesToIgnore,
        DateTime initialDateTime,
        string? initialNote = null
    )
    {
        DatetimesToIgnore = datetimesToIgnore;
        note = initialNote ?? string.Empty;
        failureDateTime = initialDateTime;

        DatePicker.SelectedDate = new DateTimeOffset(initialDateTime);
        DatePicker.SelectedDateChanged += (_, a) => UpdateFailureDate(a.NewDate.Value);

        TimePicker.ClockIdentifier = "24HourClock";
        TimePicker.SelectedTime = initialDateTime.TimeOfDay;
        TimePicker.SelectedTimeChanged += (_, a) => UpdateFailureTime(a.NewTime.Value);

        var okEnabled = this.WhenAnyValue(
            x => x.FailureDateTime,
            x => !DatetimesToIgnore.Contains(x)
        );

        Ok = ReactiveCommand.Create(() => new Failure(FailureDateTime, Note), okEnabled);
        Cancel = ReactiveCommand.Create(() => { });

    }

    public ReactiveCommand<Unit, Failure> Ok { get; }
    public ReactiveCommand<Unit, Unit> Cancel { get; }

    void UpdateFailureDate(DateTimeOffset date)
    {
        FailureDateTime = new DateTime(
            date.Year, date.Month, date.Day,
            FailureDateTime.Hour, FailureDateTime.Minute, 0
        );
    }

    void UpdateFailureTime(TimeSpan time)
    {
        FailureDateTime = new DateTime(
            FailureDateTime.Year, FailureDateTime.Month, FailureDateTime.Day,
            time.Hours, time.Minutes, 0
        );
    }

}
