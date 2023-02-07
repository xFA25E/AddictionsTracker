using System;
using System.Linq;
using AddictionsTracker.Models;
using Avalonia.Controls;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections;

namespace AddictionsTracker.Controls;

public partial class ScaleControl : UserControl
{
    static DateOnly now = DateTime.Now.ToDateOnly();
    DateOnly nextMonthToInsert = new DateOnly(now.Year, now.Month, 1);

    public ScaleControl() => InitializeComponent();
    public ScaleControl(ObservableCollection<Addiction> addictions) : this()
    {
        addictions.CollectionChanged += (s, a) =>
        {
            if (a.Action == NotifyCollectionChangedAction.Add
                && a.NewItems is IList items)
                foreach (Addiction addiction in items)
                    addiction.Failures.CollectionChanged += (_, a) =>
                        failuresCollectionChangedHandler(addictions, a);
            else if (a.Action == NotifyCollectionChangedAction.Remove)
                removeSuperfluousEelements(addictions);
        };
    }

    void failuresCollectionChangedHandler(
        ObservableCollection<Addiction> addictions,
        NotifyCollectionChangedEventArgs args
    )
    {
        if (args.Action == NotifyCollectionChangedAction.Add
            && args.NewItems is IList newItems)
            {
                DateOnly? minDate = null;
                foreach (Failure failure in newItems)
                    if (failure.FailedAt <= nextMonthToInsert
                        && (minDate == null || failure.FailedAt < minDate))
                        minDate = failure.FailedAt;
                if (minDate == null) return;

                do
                {
                    var days = now.Subtract(nextMonthToInsert);
                    var label = new ScaleLabelControl(nextMonthToInsert, days);
                    var bar = new ScaleBarControl(days);
                    canvas.Children.Add(label);
                    canvas.Children.Add(bar);
                    nextMonthToInsert = nextMonthToInsert.AddMonths(-1);
                } while (minDate <= nextMonthToInsert);
            }
        else if (args.Action == NotifyCollectionChangedAction.Remove)
        {
            removeSuperfluousEelements(addictions);
        }
    }

    void removeSuperfluousEelements(ObservableCollection<Addiction> addictions)
    {
        var nextMonth = nextMonthToInsert.AddMonths(1);
        var minDate = addictions
            .Select(a => a.Failures.LastOrDefault()?.FailedAt)
            .Min();
        if (minDate == null)
        {
            nextMonthToInsert = new DateOnly(now.Year, now.Month, 1);
            canvas.Children.Clear();
        }
        else
            while (nextMonth < minDate)
            {
                canvas.Children.RemoveAt(canvas.Children.Count - 1);
                canvas.Children.RemoveAt(canvas.Children.Count - 1);
                nextMonthToInsert = nextMonth;
                nextMonth = nextMonthToInsert.AddMonths(1);
            }
    }
}
