using System;
using AddictionsTracker.Models;
using ReactiveUI;

namespace AddictionsTracker.ViewModels;

public class FailureViewModel : ViewModelBase
{
    public Addiction Addiction { get; }
    public Failure Failure { get; }

    public FailureViewModel(
        Addiction addiction,
        Failure failure,
        DateOnly abstainedUntil
    )
    {
        Addiction = addiction;
        Failure = failure;
        AbstainedUntil = abstainedUntil;
    }

    DateOnly abstainedUntil = DateTime.Now.ToDateOnly();
    public DateOnly AbstainedUntil
    {
        get => abstainedUntil;
        set
        {
            if (abstainedUntil < Failure.FailedAt)
                throw new ArgumentException("AbstainedUntil must be greater than Failures.FailedAt", nameof(AbstainedUntil));

            this.RaiseAndSetIfChanged(ref abstainedUntil, value);
            AbstinenceDays = AbstainedUntil.Subtract(Failure.FailedAt) - 1;
        }
    }

    int abstinenceDays = 0;
    public int AbstinenceDays
    {
        get => abstinenceDays;
        private set => this.RaiseAndSetIfChanged(ref abstinenceDays, value);
    }
}
