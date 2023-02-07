using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace AddictionsTracker.Models;

public class Addiction : INotifyPropertyChanged
{

    public int Id { get; }
    string title;

    static DescendingFailureComparer comparer = new();
    public ObservableCollection<Failure> Failures { get; } = new();

    public Addiction(int id, string title)
    {
        Id = id;
        this.title = title;
    }

    int search(Failure failure) => Failures.BinarySearch(failure, comparer);

    public bool InsertFailure(Failure failure)
    {
        var i = search(failure);
        var isNotFound = int.IsNegative(i);
        if (isNotFound) Failures.Insert(~i, failure);
        return isNotFound;
    }

    public void UpdateFailure(Failure failure, DateOnly failedAt, string note)
    {
        if (!DeleteFailure(failure)) return;
        failure.FailedAt = failedAt;
        failure.Note = note;
        Failures.Insert(~search(failure), failure);
    }

    public bool DeleteFailure(Failure failure)
    {
        var i = search(failure);
        var isFound = !int.IsNegative(i);
        if (isFound) Failures.RemoveAt(i);
        return isFound;
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
