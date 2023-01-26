using System.Collections.Generic;

namespace AddictionsTracker.Models;

public class Addiction
{
    public int Id { get; }
    public string Title { get; set; }

    public SortedSet<Failure> Failures { get; } =
        new SortedSet<Failure>(new DescendingFailureComparer());

    public Addiction(int id, string title)
    {
        Id = id;
        Title = title;
    }
}
