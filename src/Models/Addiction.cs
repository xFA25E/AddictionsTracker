using System;
using System.Collections.Generic;

namespace AddictionsTracker.Models;

public class Addiction
{
    public string Title { get; set; }
    public SortedSet<Failure> Failures { get; } =
        new SortedSet<Failure>(new FailureComparer());

    public Addiction(string title)
    {
        Title = title;
    }
}
