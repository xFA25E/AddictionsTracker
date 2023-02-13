using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AddictionsTracker.Models;

namespace AddictionsTracker.ViewModels;

public class FailuresViewModel : ViewModelBase
{
    public Addiction Addiction { get; }
    public ObservableCollection<FailureViewModel> Failures { get; } = new();

    public FailuresViewModel(Addiction addiction, IEnumerable<Failure> failures)
    {
        Addiction = addiction;
        foreach (var failure in failures)
            InsertFailure(failure);
    }

    public bool InsertFailure(Failure failure)
    {
        var i = BinarySearch(failure);
        var isNotFound = int.IsNegative(i);
        if (isNotFound)
        {
            i = ~i;
            var abstainedUntil = Failures.Count == 0
                ? DateTime.Now.ToDateOnly().AddDays(1)
                : (i < Failures.Count
                   ? Failures[i].AbstainedUntil
                   : Failures[i - 1].Failure.FailedAt);

            var failureVM = new FailureViewModel(
                Addiction,
                failure,
                abstainedUntil
            );

            Failures.Insert(i, failureVM);
            if (i + 1 < Failures.Count)
                Failures[i + 1].AbstainedUntil = failure.FailedAt;
        }
        return isNotFound;
    }

    public void UpdateFailure(Failure failure, DateOnly failedAt, string note)
    {
        if (!DeleteFailure(failure)) return;
        failure.FailedAt = failedAt;
        failure.Note = note;
        InsertFailure(failure);
    }

    public bool DeleteFailure(Failure failure)
    {
        var i = BinarySearch(failure);
        var isFound = !int.IsNegative(i);
        if (isFound)
        {
            var abstainedUntil = Failures[i].AbstainedUntil;
            Failures.RemoveAt(i);
            if (i < Failures.Count)
                Failures[i].AbstainedUntil = abstainedUntil;
        }
        return isFound;
    }

    static readonly DescendingFailureComparer comparer = new();
    public int BinarySearch(Failure failure)
    {
        int lower = 0;
        int upper = Failures.Count - 1;

        while (lower <= upper)
        {
            int middle = lower + (upper - lower) / 2;
            int comparisonResult = comparer.Compare(failure, Failures[middle].Failure);
            if (comparisonResult == 0)
                return middle;
            else if (comparisonResult < 0)
                upper = middle - 1;
            else
                lower = middle + 1;
        }

        return ~lower;
    }
}
