using System;
using System.Collections.Generic;

namespace AddictionsTracker.Models;

public class Failure
{
    public int Id { get; }
    public DateOnly FailedAt { get; set; }
    public string Note { get; set; }

    public Failure(int id, DateOnly failedAt, string note)
    {
        Id = id;
        FailedAt = failedAt;
        Note = note;
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
