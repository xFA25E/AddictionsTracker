using System;
using System.Collections.Generic;
using System.Reactive;
using ReactiveUI;

namespace AddictionsTracker.ViewModels;

public class ConfirmationDialogWindowViewModel : ViewModelBase
{
    public string Prompt { get; }

    public ConfirmationDialogWindowViewModel(string prompt)
    {
        Prompt = prompt;
        YesCommand = ReactiveCommand.Create(() => {});
        NoCommand = ReactiveCommand.Create(() => { });
    }

    public ReactiveCommand<Unit, Unit> YesCommand { get; }
    public ReactiveCommand<Unit, Unit> NoCommand { get; }
}
