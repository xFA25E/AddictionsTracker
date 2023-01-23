using System;
using System.Collections.Generic;

namespace AddictionsTracker.Models;

public class Failure
{
    public DateTime FailedAt { get; }
    public string? Note { get; set; }

    public Failure(DateTime failedAt, string? note = null)
    {
        FailedAt = failedAt;
        Note = note;
    }
}

public class FailedAtComparer : Comparer<DateTime>
{
    public override int Compare(DateTime x, DateTime y)
    {

        if (y.Year.CompareTo(x.Year) != 0)
        {
            return y.Year.CompareTo(x.Year);
        }
        else if (y.Month.CompareTo(x.Month) != 0)
        {
            return y.Month.CompareTo(x.Month);
        }
        else if (y.Day.CompareTo(x.Day) != 0)
        {
            return y.Day.CompareTo(x.Day);
        }
        else if (y.Hour.CompareTo(x.Hour) != 0)
        {
            return y.Hour.CompareTo(x.Hour);
        }
        else if (y.Minute.CompareTo(x.Minute) != 0)
        {
            return y.Minute.CompareTo(x.Minute);
        }

        return 0;
    }
}

public class FailureComparer : Comparer<Failure>
{
    public override int Compare(Failure? x, Failure? y)
    {
        if (x == null || y == null)
        {
            throw new ArgumentException("Cannot compare Failure to null!");
        }

        return (new FailedAtComparer()).Compare(x.FailedAt, y.FailedAt);
    }
}
