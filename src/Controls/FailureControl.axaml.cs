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
        border.Tapped += (_, _) => border.ContextMenu?.Open();
    }
}

public class FailureControlViewModel : INotifyPropertyChanged
{
    public Addiction Addiction { get; }
    public Failure Failure { get; }

    public int AbstinenceDays => AbstainedUntil.Subtract(Failure.FailedAt) - 1;
    public int AbstinenceWidth => AbstinenceDays * DayWidth.Instance.Width;
    public int FailureWidth => DayWidth.Instance.Width - 1;

    public FailureControlViewModel(
        Addiction addiction,
        Failure failure,
        DateOnly abstainedUntil
    )
    {
        this.abstainedUntil = abstainedUntil;

        Addiction = addiction;
        Failure = failure;

        DayWidth.Instance.PropertyChanged += dayWidthChangedHandler;
        Failure.PropertyChanged += failureChangedHandler;
    }

    void dayWidthChangedHandler(object? sender, PropertyChangedEventArgs args)
    {
        if (args.PropertyName?.Equals(nameof(DayWidth.Instance.Width)) != null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AbstinenceWidth)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FailureWidth)));
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
