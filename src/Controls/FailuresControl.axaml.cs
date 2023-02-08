using System;
using System.Collections.Specialized;
using AddictionsTracker.Models;
using Avalonia.Controls;

namespace AddictionsTracker.Controls;

public partial class FailuresControl : UserControl
{
    public FailuresControl() => InitializeComponent();
    public FailuresControl(
        Addiction addiction,
        Action<Addiction, Failure> updateFailure,
        Action<Addiction, Failure> deleteFailure
    ) : this()
    {
        addiction.Failures.CollectionChanged += (s, a) =>
        {
            if (a.Action == NotifyCollectionChangedAction.Add)
            {
                var i = a.NewStartingIndex;
                var failure = addiction.Failures[i];
                var abstainedUntil =
                    (1 == addiction.Failures.Count)
                    ? Globals.Now.AddDays(1)
                    : ((0 == i)
                       ? childDataContext(0).AbstainedUntil
                       : addiction.Failures[i - 1].FailedAt);

                var failureControl = new FailureControl(
                    addiction,
                    failure,
                    abstainedUntil,
                    updateFailure,
                    deleteFailure
                );

                failures.BeginBatchUpdate();
                if (i != failures.Children.Count)
                    childDataContext(i).AbstainedUntil = failure.FailedAt;
                failures.Children.Insert(i, failureControl);
                failures.EndBatchUpdate();
            }
            else if (a.Action == NotifyCollectionChangedAction.Remove)
            {
                var i = a.OldStartingIndex;
                var abstainedUntil = childDataContext(i).AbstainedUntil;

                failures.BeginBatchUpdate();
                failures.Children.RemoveAt(i);

                if (i < failures.Children.Count)
                    childDataContext(i).AbstainedUntil = abstainedUntil;
                failures.EndBatchUpdate();
            }
        };
    }

    FailureControlViewModel childDataContext(int i)
    {
        var control = (FailureControl)failures.Children[i];
        if (control.DataContext is FailureControlViewModel dc)
            return dc;

        throw new InvalidOperationException("FailureControl's DataContext was not initialized properly");
    }
}
