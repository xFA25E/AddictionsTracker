using System;
using System.ComponentModel;
using AddictionsTracker.Models;
using Avalonia.Controls;

namespace AddictionsTracker.Controls;

public partial class ScaleLabelControl : UserControl
{
    public ScaleLabelControl() => InitializeComponent();
    public ScaleLabelControl(DateOnly date, int days) : this()
    {
        DataContext = new ScaleLabelControlViewModel(date, days);
    }
}

public class ScaleLabelControlViewModel : INotifyPropertyChanged
{
    int days;
    public DateOnly Date { get; }
    public int Left => days * DayWidth.Instance.Width;

    public ScaleLabelControlViewModel(DateOnly date, int days)
    {
        this.days = days;
        Date = date;

        DayWidth.Instance.PropertyChanged += (_, a) =>
        {
            if (a.PropertyName is string property
                && property.Equals(nameof(DayWidth.Instance.Width)))
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Left)));
        };
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}
