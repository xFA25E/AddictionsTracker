using System.Collections.Generic;
using System.Collections.ObjectModel;
using AddictionsTracker.Models;

namespace AddictionsTracker.ViewModels;

public class AddictionsListViewModel : ViewModelBase
{
    public AddictionsListViewModel(IEnumerable<Addiction> addictions)
    {
        Addictions = new ObservableCollection<Addiction>(addictions);
    }

    public ObservableCollection<Addiction> Addictions { get; }
}
