using System.Collections.Generic;
using AddictionsTracker.Models;

namespace AddictionsTracker.ViewModels;

public class AddictionViewModel : ViewModelBase
{
    public Addiction Addiction { get; }
    public FailuresViewModel Failures { get; }

    public AddictionViewModel(Addiction addiction, IEnumerable<Failure> failures)
    {
        Addiction = addiction;
        Failures = new(addiction, failures);
    }
}
