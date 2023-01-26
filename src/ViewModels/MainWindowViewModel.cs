using ReactiveUI;
using System.Reactive.Linq;
using System;
using AddictionsTracker.Services;
using AddictionsTracker.Models;
using System.Linq;
using System.Collections.Generic;

namespace AddictionsTracker.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    Database Database { get; }

    ViewModelBase content;
    public ViewModelBase Content
    {
        get => content;
        set => this.RaiseAndSetIfChanged(ref content, value);
    }

    public AddictionsListViewModel AddictionsList { get; }

    public MainWindowViewModel(Database database)
    {
        AddictionsList = new AddictionsListViewModel(database.GetAddictions());
        Content = AddictionsList;
        content = Content;
        Database = database;
    }

    public void InsertAddiction()
    {
        HandleAddictionDialog(
            string.Empty,
            addictionTitle =>
            {
                if (addictionTitle != null)
                {
                    AddictionsList.Addictions.Add(
                        Database.InsertAddiction(addictionTitle)
                    );
                }
            }
        );
    }

    public void UpdateAddiction(int addictionId)
    {
        var addiction = findAddiction(addictionId);

        HandleAddictionDialog(
            addiction.Title,
            addictionTitle =>
            {
                if (addictionTitle != null)
                {
                    Database.UpdateAddiction(addiction, addictionTitle);
                    addiction.Title = addictionTitle;
                }
            }
        );
    }

    public void DeleteAddiction(int addictionId)
    {
        var addiction = findAddiction(addictionId);

        HandleDeletionDialog(
            string.Format(
                "Are you sure that you want to delete the following addiction: {0}",
                addiction.Title
            ),
            isYes =>
            {
                if (isYes)
                {
                    Database.DeleteAddiction(addiction);
                    AddictionsList.Addictions.Remove(addiction);
                }
            }
        );
    }

    public void InsertFailure(int addictionId)
    {
        var addiction = findAddiction(addictionId);

        var now = DateTime.Now;
        HandleFailureDialog(
            addiction.Failures,
            new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0),
            string.Empty,
            failureTuple =>
            {
                if (failureTuple != null)
                {
                    var (failedAt, note) = failureTuple.Value;
                    addiction.Failures.Add(
                        Database.InsertFailure(addiction, failedAt, note)
                    );
                }
            }
        );
    }

    public void UpdateFailure(int failureId)
    {
        var (addiction, failure) = findAddictionAndFailure(failureId);

        HandleFailureDialog(
            addiction.Failures,
            failure.FailedAt,
            failure.Note,
            failureTuple =>
            {
                if (failureTuple != null)
                {
                    var (failedAt, note) = failureTuple.Value;

                    Database.UpdateFailure(failure, failedAt, note);
                    addiction.Failures.Remove(failure);
                    failure.FailedAt = failedAt;
                    failure.Note = note;
                    addiction.Failures.Add(failure);
                }
            }
        );
    }

    public void DeleteFailure(int failureId)
    {
        var (addiction, failure) = findAddictionAndFailure(failureId);

        HandleDeletionDialog(
            string.Format(
                "Are you sure that you want to delete the following failure of \"{0}\" addiction: {1} {2}",
                addiction.Title,
                failure.FailedAt.ToString("u"),
                failure.Note
            ),
            isYes =>
            {
                if (isYes)
                {
                    addiction.Failures.Remove(failure);
                    Database.DeleteFailure(failure);
                }
            }
        );
    }

    void HandleAddictionDialog(string initialTitle, Action<string?> callback)
    {
        var titles = AddictionsList.Addictions
            .Select(a => a.Title)
            .Where(t => !t.Equals(initialTitle))
            .ToHashSet();

        var vm = new AddictionDialogViewModel(titles, initialTitle);

        Observable.Merge(
            vm.Ok,
            vm.Cancel.Select(_ => (string?)null)
        ).Take(1).Subscribe(
            addictionTitle =>
            {
                callback(addictionTitle);
                Content = AddictionsList;
            }
        );

        Content = vm;
    }

    void HandleFailureDialog(
        IEnumerable<Failure> failures,
        DateTime initialDateTime,
        string initialNote,
        Action<(DateTime, string)?> callback
    )
    {
        var failedAts = new SortedSet<DateTime>(
            failures
            .Select(f => f.FailedAt)
            .Where(d => !d.Equals(initialDateTime)),
            new DescendingFailedAtComparer()
        );

        var vm = new FailureDialogViewModel(
            failedAts,
            initialDateTime,
            initialNote
        );

        Observable.Merge(
            vm.Ok.Select(t => ((DateTime, string)?)t),
            vm.Cancel.Select(_ => ((DateTime, string)?)null)
        ).Take(1).Subscribe(
            failureTuple =>
            {
                callback(failureTuple);
                Content = AddictionsList;
            }
        );

        Content = vm;
    }

    void HandleDeletionDialog(string text, Action<bool> callback)
    {
        var vm = new DeletionDialogViewModel(text);

        Observable.Merge(
            vm.Yes.Select(_ => true),
            vm.No.Select(_ => false)
        ).Take(1).Subscribe(
            isYes =>
            {
                callback(isYes);
                Content = AddictionsList;
            }
        );

        Content = vm;
    }

    Addiction findAddiction(int addictionId)
    {
        return AddictionsList.Addictions.Single(a => a.Id == addictionId);
    }

    (Addiction, Failure) findAddictionAndFailure(int failureId)
    {
        foreach (var a in AddictionsList.Addictions)
        {
            foreach (var f in a.Failures)
            {
                if (f.Id == failureId)
                {
                    return (a, f);
                }
            }
        }
        throw new ArgumentException("Invalid id", nameof(failureId));
    }
}
