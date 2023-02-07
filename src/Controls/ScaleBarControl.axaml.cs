using System.ComponentModel;
using AddictionsTracker.Models;
using Avalonia.Controls;

namespace AddictionsTracker.Controls;

public partial class ScaleBarControl : UserControl
{
    public ScaleBarControl() => InitializeComponent();
    public ScaleBarControl(int days) : this()
    {
        DataContext = new ScaleBarControlViewModel(days);
    }
}

public class ScaleBarControlViewModel : INotifyPropertyChanged
{
    int days;
    public DayWidth DayWidth => DayWidth.Instance;
    public int Left => DayWidth.Width * days;

    public ScaleBarControlViewModel(int days)
    {
        this.days = days;
        DayWidth.PropertyChanged += (_, a) =>
        {
            if (a.PropertyName is string property
                && property.Equals(nameof(DayWidth.Width)))
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Left)));
        };
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}
