using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using AddictionsTracker.ViewModels;
using Avalonia.Controls;

namespace AddictionsTracker.Views;

public partial class FailuresView : UserControl
{
    public FailuresView()
    {
        InitializeComponent();

        PropertyChanged += (s, a) =>
        {
            if (a.Property.Name.Equals("DataContext")
                && a.NewValue is FailuresViewModel fsVM)
            {
                failures.Children.AddRange(
                    fsVM.Failures.Select(
                        fVM => new ContentControl() { Content = fVM }
                    )
                );

                fsVM.Failures.CollectionChanged += onCollectionChanged;
            }
        };
    }

    void onCollectionChanged(object? sender, NotifyCollectionChangedEventArgs a)
    {
        if (a.Action == NotifyCollectionChangedAction.Add
            && a.NewItems is IList newItems
            && newItems[0] is FailureViewModel fVM)
        {
            failures.Children.Insert(
                a.NewStartingIndex,
                new ContentControl() { Content = fVM }
            );
        }
        else if (a.Action == NotifyCollectionChangedAction.Remove)
        {
            failures.Children.RemoveAt(a.OldStartingIndex);
        }
    }
}
