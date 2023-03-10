using System;
using System.ComponentModel;
using AddictionsTracker.Models;
using Avalonia.Controls;

namespace AddictionsTracker.Controls;

public partial class FailureControl : UserControl
{
    public FailureControl() => InitializeComponent();
    public FailureControl(
        Addiction addiction,
        Failure failure,
        DateOnly abstainedUntil,
        Action<Addiction, Failure> update,
        Action<Addiction, Failure> delete
    ) : this()
    {
        DataContext = new FailureControlViewModel(
            addiction,
            failure,
            abstainedUntil
        );

        edit.Click += (_, _) => update(addiction, failure);
        remove.Click += (_, _) => delete(addiction, failure);
        button.Tapped += (_, _) => button.ContextMenu?.Open();
    }
}

public class FailureControlViewModel : INotifyPropertyChanged
{
    public Addiction Addiction { get; }
    public Failure Failure { get; }

    public DayWidth DayWidth => Globals.DayWidth;

    public int AbstinenceDays => AbstainedUntil.Subtract(Failure.FailedAt) - 1;
    public int AbstinenceWidth => AbstinenceDays * DayWidth.Width;

    public FailureControlViewModel(
        Addiction addiction,
        Failure failure,
        DateOnly abstainedUntil
    )
    {
        this.abstainedUntil = abstainedUntil;

        Addiction = addiction;
        Failure = failure;

        Globals.DayWidth.PropertyChanged += dayWidthChangedHandler;
        Failure.PropertyChanged += failureChangedHandler;
    }

    void dayWidthChangedHandler(object? sender, PropertyChangedEventArgs args)
    {
        if (args.PropertyName?.Equals(nameof(Globals.DayWidth.Width)) != null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AbstinenceWidth)));
        }
    }

    void failureChangedHandler(object? sender, PropertyChangedEventArgs args)
    {
        if (args.PropertyName?.Equals(nameof(Failure.FailedAt)) != null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AbstinenceDays)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AbstinenceWidth)));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    DateOnly abstainedUntil;
    public DateOnly AbstainedUntil
    {
        get => abstainedUntil;
        set
        {
            if (!abstainedUntil.Equals(value))
            {
                abstainedUntil = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AbstainedUntil)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AbstinenceDays)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AbstinenceWidth)));
            }
        }
    }
}
