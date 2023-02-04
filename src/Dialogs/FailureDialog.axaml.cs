using System;
using System.Collections.Generic;
using System.ComponentModel;
using Avalonia.Controls;

namespace AddictionsTracker.Dialogs;

public partial class FailureDialog : Window
{
    public FailureDialog()
        : this(
            "ADDICTION",
            DateOnly.FromDateTime(DateTime.Now),
            string.Empty,
            new DateOnly[0]
        ) {}

    public FailureDialog(
        string addictionTitle,
        DateOnly initialDate,
        string initialNote,
        IEnumerable<DateOnly> forbiddenDates
    )
    {
        InitializeComponent();
        DataContext = new FailureDialogViewModel(forbiddenDates)
        {
            AddictionTitle = addictionTitle,
            Note = initialNote,
            Ok = (data) => this.Close(data),
            Cancel = () => this.Close(null),
        };
        datePicker.SelectedDate = initialDate.ToDateTimeOffset();
    }

    public void datePicker_SelectedDateChanged(
        object? sender,
        DatePickerSelectedValueChangedEventArgs args
    )
    {
        if (DataContext != null && args.NewDate != null)
        {
            ((FailureDialogViewModel)DataContext).FailedAt =
                args.NewDate.Value.ToDateOnly();
        }
    }
}

public class FailureDialogViewModel : INotifyPropertyChanged
{
    DateOnly now = DateTime.Now.ToDateOnly();
    HashSet<DateOnly> forbiddenDates;

    public FailureDialogViewModel(IEnumerable<DateOnly> forbiddenDates)
    {
        this.forbiddenDates = new(forbiddenDates);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    string addictionTitle = "ADDICTION";
    public string AddictionTitle
    {
        get => addictionTitle;
        set
        {
            addictionTitle = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AddictionTitle)));
        }
    }

    DateOnly failedAt = DateTime.Now.ToDateOnly();
    public DateOnly FailedAt
    {
        get => failedAt;
        set
        {
            failedAt = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FailedAt)));
            CanOk = now >= value && !forbiddenDates.Contains(value);
        }
    }

    string note = string.Empty;
    public string Note
    {
        get => note;
        set
        {
            note = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Note)));
        }
    }

    void OkCommand() => Ok((FailedAt, Note));
    Action<(DateOnly, string)> ok = (_) => {};
    public Action<(DateOnly, string)> Ok {
        get => ok;
        set
        {
            ok = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Ok)));
        }
    }

    bool canOk = false;
    public bool CanOk
    {
        get => canOk;
        set
        {
            canOk = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanOk)));
        }
    }

    Action cancel = () => {};
    public Action Cancel {
        get => cancel;
        set
        {
            cancel = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Cancel)));
        }
    }
}
