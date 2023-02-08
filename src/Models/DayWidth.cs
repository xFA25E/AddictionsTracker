using System.ComponentModel;

namespace AddictionsTracker.Models;

public class DayWidth : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    int width = 3;
    public int Width
    {
        get => width;
        set
        {
            if (width != value && 2 <= value)
            {
                width = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Width)));
            }
        }
    }
}
