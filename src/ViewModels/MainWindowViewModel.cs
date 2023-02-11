using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;
using AddictionsTracker.Models;
using AddictionsTracker.Services;
using ReactiveUI;

namespace AddictionsTracker.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    Database database = new Database();
    ObservableCollection<AddictionViewModel> Addictions { get; } = new();

    int dayWidth = 3;
    public int DayWidth
    {
        get => dayWidth;
        set
        {
            if (dayWidth != value && 2 <= value)
            {
                dayWidth = value;
                this.RaiseAndSetIfChanged(ref dayWidth, value);
            }
        }
    }

    public MainWindowViewModel()
    {
        foreach (var kvp in database.GetAddictions())
            Addictions.Add(new(kvp.Key, kvp.Value));

        ShowAddictionDialog = new Interaction<AddictionDialogWindowViewModel, string?>();
        ShowConfirmationDialog = new Interaction<ConfirmationDialogWindowViewModel, bool>();
        ShowFailureDialog = new Interaction<FailureDialogWindowViewModel, (DateOnly, string)?>();

        InsertAddictionCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var addictionDialog = new AddictionDialogWindowViewModel(
                "",
                Addictions.Select(a => a.Addiction.Title)
            );
            var result = await ShowAddictionDialog.Handle(addictionDialog);
            if (result is string title)
            {
                var addiction = database.InsertAddiction(title);
                Addictions.Add(new AddictionViewModel(addiction, new Failure[0]));
            }
        });

        UpdateAddictionCommand = ReactiveCommand.CreateFromTask<Addiction>(async addiction =>
        {
            var addictionDialog = new AddictionDialogWindowViewModel(
                addiction.Title,
                Addictions .Select(a => a.Addiction.Title)
                .Where(t => !t.Equals(addiction.Title))
            );
            var result = await ShowAddictionDialog.Handle(addictionDialog);
            if (result is string title)
            {
                database.UpdateAddiction(addiction, title);
                addiction.Title = title;
            }
        });

        DeleteAddictionCommand = ReactiveCommand.CreateFromTask<Addiction>(async addiction =>
        {
            var confirmationDialog = new ConfirmationDialogWindowViewModel(
                $"Are you sure that you want to delete {addiction.Title}?"
            );
            var result = await ShowConfirmationDialog.Handle(confirmationDialog);
            if (result)
            {
                var aVM = Addictions.Single(a => a.Addiction.Id == addiction.Id);
                database.DeleteAddiction(addiction);
                Addictions.Remove(aVM);
            }
        });

        MoveUpAddictionCommand = ReactiveCommand.Create<Addiction>(addiction =>
        {
            var aVM = Addictions.Single(a => a.Addiction.Id == addiction.Id);
            var row = Addictions.IndexOf(aVM);
            if (0 < row)
            {
                var temp = Addictions[row];
                Addictions[row] = Addictions[row - 1];
                Addictions[row - 1] = temp;
            }
        });

        MoveDownAddictionCommand = ReactiveCommand.Create<Addiction>(addiction =>
        {
            var aVM = Addictions.Single(a => a.Addiction.Id == addiction.Id);
            var row = Addictions.IndexOf(aVM);
            if (row < Addictions.Count - 1)
            {
                var temp = Addictions[row];
                Addictions[row] = Addictions[row + 1];
                Addictions[row + 1] = temp;
            }
        });

        InsertFailureCommand = ReactiveCommand.CreateFromTask<Addiction>(async addiction =>
        {
            var aVM = Addictions.Single(a => a.Addiction.Id == addiction.Id);
            var failureDialog = new FailureDialogWindowViewModel(
                DateTime.Now.ToDateOnly(),
                "",
                aVM.Failures.Select(f => f.Failure.FailedAt)
            );
            var result = await ShowFailureDialog.Handle(failureDialog);
            if (result is (DateOnly failedAt, string note))
            {
                var failure = database.InsertFailure(addiction, failedAt, note);
                aVM.InsertFailure(failure);
            }
        });

        UpdateFailureCommand = ReactiveCommand.CreateFromTask<ReadOnlyCollection<Object>>(async coll =>
        {
            if (coll.ElementAtOrDefault(0) is Addiction addiction
                && coll.ElementAtOrDefault(1) is Failure failure)
            {
                var aVM = Addictions.Single(a => a.Addiction.Id == addiction.Id);
                var failureDialog = new FailureDialogWindowViewModel(
                    failure.FailedAt,
                    failure.Note,
                    aVM.Failures.Select(f => f.Failure.FailedAt)
                    .Where(d => !d.Equals(failure.FailedAt))
                );
                var result = await ShowFailureDialog.Handle(failureDialog);
                if (result is (DateOnly failedAt, string note))
                {
                    database.UpdateFailure(failure, failedAt, note);
                    aVM.UpdateFailure(failure, failedAt, note);
                }
            }
        });

        DeleteFailureCommand = ReactiveCommand.CreateFromTask<ReadOnlyCollection<Object>>(async coll =>
        {
            if (coll.ElementAtOrDefault(0) is Addiction addiction
                && coll.ElementAtOrDefault(1) is Failure failure)
            {
                var confirmationDialog = new ConfirmationDialogWindowViewModel(
                    $"Are you sure that you want to delete failure of {addiction.Title} at {failure.FailedAt:yyyy MMMMM dd}?"
                );
                var result = await ShowConfirmationDialog.Handle(confirmationDialog);
                if (result)
                {
                    var aVM = Addictions.Single(a => a.Addiction.Id == addiction.Id);
                    database.DeleteFailure(failure);
                    aVM.DeleteFailure(failure);
                }
            }
        });
    }

    public Interaction<AddictionDialogWindowViewModel, string?> ShowAddictionDialog { get; }
    public Interaction<ConfirmationDialogWindowViewModel, bool> ShowConfirmationDialog { get; }
    public Interaction<FailureDialogWindowViewModel, (DateOnly, string)?> ShowFailureDialog { get; }

    public ICommand InsertAddictionCommand { get; }
    public ICommand UpdateAddictionCommand { get; }
    public ICommand DeleteAddictionCommand { get; }
    public ICommand MoveUpAddictionCommand { get; }
    public ICommand MoveDownAddictionCommand { get; }

    public ICommand InsertFailureCommand { get; }
    public ICommand UpdateFailureCommand { get; }
    public ICommand DeleteFailureCommand { get; }
}
