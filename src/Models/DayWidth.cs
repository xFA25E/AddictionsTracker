using System.ComponentModel;

namespace AddictionsTracker.Models;

public class DayWidth : INotifyPropertyChanged
{
    public static DayWidth Instance = new DayWidth();

    public event PropertyChangedEventHandler? PropertyChanged;

    int width = 3;
    public int Width
    {
        get => width;
        set
        {
            if (value >= 2)
            {
                width = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Width)));
            }
        }
    }
}
