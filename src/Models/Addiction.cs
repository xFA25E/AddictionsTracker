using System.Collections.Generic;
using System.ComponentModel;

namespace AddictionsTracker.Models;

public class Addiction : INotifyPropertyChanged
{
    public int Id { get; }
    string title;

    public SortedSet<Failure> Failures { get; } =
        new(new DescendingFailureComparer());

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
            title = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Title)));
        }
    }
}
