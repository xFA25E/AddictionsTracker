using System;
using System.ComponentModel;
using AddictionsTracker.Models;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace AddictionsTracker.Controls;

public partial class FailureControl : UserControl
{
    public FailureControl() => InitializeComponent();
    public FailureControl(
        Addiction addiction,
        Failure failure,
        DateOnly abstainedUntil,
        Action update,
        Action delete
    ) : this()
    {
        DataContext = new FailureControlViewModel(
            addiction,
            failure,
            abstainedUntil
        )
        {
            Update = update,
            Delete = delete,
        };
    }

    void border_Tapped(object? sender, RoutedEventArgs args)
    {
        if (sender is Border border) border.ContextMenu?.Open();
    }
}

public class FailureControlViewModel : INotifyPropertyChanged
{
    Addiction addiction;
    Failure failure;
    DateOnly abstainedUntil;

    public event PropertyChangedEventHandler? PropertyChanged;

    public FailureControlViewModel(
        Addiction addiction,
        Failure failure,
        DateOnly abstainedUntil
    )
    {
        this.addiction = addiction;
        this.failure = failure;
        this.abstainedUntil = abstainedUntil;

        this.addiction.PropertyChanged += PropertyChangedHandler;
        this.failure.PropertyChanged += PropertyChangedHandler;
        DayWidth.Instance.PropertyChanged += PropertyChangedHandler;
    }

    void PropertyChangedHandler(object? sender, PropertyChangedEventArgs a)
    {
        switch (a.PropertyName)
        {
            case nameof(addiction.Title):
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FailureTooltipHeader)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AbstinenceTooltip)));
                break;
            case nameof(failure.FailedAt):
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FailureTooltipHeader)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AbstinenceDays)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AbstinenceTooltip)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AbstinenceWidth)));
                break;
            case nameof(failure.Note):
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FailureTooltipNote)));
                break;
            case nameof(DayWidth.Instance.Width):
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FailureWidth)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AbstinenceWidth)));
                break;
        }
    }

    public string FailureTooltipHeader => $"At {failure.FailedAt.ToString("yyyy-MM-dd")} failed {addiction.Title}";
    public string FailureTooltipNote => failure.Note;
    public int FailureWidth => DayWidth.Instance.Width - 1;

    public int AbstinenceDays => abstainedUntil.Subtract(failure.FailedAt) - 1;
    public string AbstinenceTooltip => $"Abstained for {AbstinenceDays} day{(AbstinenceDays == 1 ? "" : "s")} from {addiction.Title}";
    public int AbstinenceWidth => AbstinenceDays * DayWidth.Instance.Width;

    public DateOnly AbstainedUntil
    {
        get => abstainedUntil;
        set
        {
            abstainedUntil = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AbstainedUntil)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AbstinenceDays)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AbstinenceTooltip)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AbstinenceWidth)));
        }
    }

    Action update = () => { };
    public Action Update
    {
        get => update;
        set
        {
            update = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Update)));
        }
    }

    Action delete = () => { };
    public Action Delete
    {
        get => delete;
        set
        {
            delete = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Delete)));
        }
    }
}
