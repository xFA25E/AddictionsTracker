using System.ComponentModel;

namespace AddictionsTracker.Models;

public class Addiction : INotifyPropertyChanged
{
    public int Id { get; }
    string title;

    public Addiction(int id, string title)
    {
        Id = id;
        this.title = title;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Title {
        get => title;
        set
        {
            if (!title.Equals(value))
            {
                title = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Title)));
            }
        }
    }
}
