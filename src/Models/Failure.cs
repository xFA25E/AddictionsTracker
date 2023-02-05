using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace AddictionsTracker.Models;

public class Failure : INotifyPropertyChanged
{
    public int Id { get; }
    DateOnly failedAt;
    string note;

    public Failure(int id, DateOnly failedAt, string note)
    {
        Id = id;
        this.failedAt = failedAt;
        this.note = note;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public DateOnly FailedAt {
        get => failedAt;
        set
        {
            failedAt = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FailedAt)));
        }
    }

    public string Note {
        get => note;
        set
        {
            note = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Note)));
        }
    }
}

public class DescendingFailureComparer : Comparer<Failure>
{
    public override int Compare(Failure? x, Failure? y)
    {
        if (x == null || y == null)
        {
            throw new ArgumentException("Cannot compare null Failures");
        }

        if (x.Id.CompareTo(y.Id) == 0)
        {
            return 0;
        }

        return y.FailedAt.CompareTo(x.FailedAt);
    }
}
