using System;
using System.Collections.Generic;

namespace AddictionsTracker.Models;

public class Failure
{
    public int Id { get; }
    public DateTime FailedAt { get; set; }
    public string Note { get; set; }

    public Failure(int id, DateTime failedAt, string note)
    {
        Id = id;
        FailedAt = failedAt;
        Note = note;
    }
}

public class DescendingFailedAtComparer : Comparer<DateTime>
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
            return x.Id.CompareTo(y.Id);
        }

        return (new DescendingFailedAtComparer())
            .Compare(x.FailedAt, y.FailedAt);
    }
}
