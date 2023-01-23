using ReactiveUI;
using System.Reactive.Linq;
using System;
using AddictionsTracker.Services;
using AddictionsTracker.Models;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

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
            addiction =>
            {
                if (addiction != null)
                {
                    Database.InsertAddiction(addiction.Title);
                    AddictionsList.Addictions.Add(addiction);
                }
            }
        );
    }

    public void UpdateAddiction(string addictionTitle)
    {

        HandleAddictionDialog(
            addictionTitle,
            addiction =>
            {
                if (addiction != null)
                {
                    Database.UpdateAddiction(addictionTitle, addiction.Title);
                    AddictionsList.Addictions.Single(
                        x => x.Title.Equals(addictionTitle)
                    ).Title = addiction.Title;
                }
            }
        );
    }

    public void DeleteAddiction(string addictionTitle)
    {
        HandleDeletionDialog(
            string.Format(
                "Are you sure that you want to delete the following addiction: {0}",
                addictionTitle
            ),
            isYes =>
            {
                if (isYes)
                {
                    Database.DeleteAddiction(addictionTitle);
                    AddictionsList.Addictions.Remove(
                        AddictionsList.Addictions.Single(
                            x => x.Title.Equals(addictionTitle)
                        )
                    );
                }
            }
        );
    }

    public void InsertFailure(string addictionTitle)
    {
        HandleFailureDialog(
            addictionTitle,
            DateTime.Now,
            string.Empty,
            failure =>
            {
                if (failure != null)
                {
                    Database.InsertFailure(
                        addictionTitle,
                        failure.FailedAt,
                        failure.Note
                    );
                    AddictionsList.Addictions.Single(
                        x => x.Title.Equals(addictionTitle)
                    ).Failures.Add(failure);
                }
            }
        );
    }

    public void UpdateFailure(object args)
    {
        var addictionTitle = (string) ((ReadOnlyCollection<Object>) args)[0];
        var failedAt = (DateTime) ((ReadOnlyCollection<Object>)args)[1];
        var note = (string?) ((ReadOnlyCollection<Object>)args)[2];
        var failures = AddictionsList.Addictions.Single(
            x => x.Title.Equals(addictionTitle)
        ).Failures;

        failures.Remove(new Failure(failedAt));

        HandleFailureDialog(
            addictionTitle,
            failedAt,
            note,
            failure =>
            {
                if (failure != null)
                {
                    Database.UpdateFailure(
                        addictionTitle,
                        failedAt,
                        failure.Note,
                        failure.FailedAt
                    );
                    failures.Add(failure);
                } else {
                    failures.Add(new Failure(failedAt, note));
                }
            }
        );
    }

    public void DeleteFailure(object args)
    {
        var addictionTitle = (string) ((ReadOnlyCollection<Object>) args)[0];
        var failedAt = (DateTime) ((ReadOnlyCollection<Object>)args)[1];
        var note = (string?) ((ReadOnlyCollection<Object>)args)[2];

        HandleDeletionDialog(
            string.Format(
                "Are you sure that you want to delete the following failure of \"{0}\" addiction: {1} {2}",
                addictionTitle,
                failedAt.ToString("u"),
                note
            ),
            isYes =>
            {
                if (isYes)
                {
                    Database.DeleteFailure(addictionTitle, failedAt);
                    AddictionsList.Addictions.Single(
                        x => x.Title.Equals(addictionTitle)
                    ).Failures.Remove(new Failure(failedAt));
                }
            }
        );
    }

    void HandleAddictionDialog(string initialTitle, Action<Addiction?> callback)
    {
        var titles = AddictionsList.Addictions.Select(x => x.Title).ToHashSet();
        var vm = new AddictionDialogViewModel(titles, initialTitle);

        Observable.Merge(vm.Ok, vm.Cancel.Select(_ => (Addiction?)null)).Take(1)
            .Subscribe(
                addiction =>
                {
                    callback(addiction);
                    Content = AddictionsList;
                }
            );

        Content = vm;
    }

    void HandleDeletionDialog(string text, Action<bool> callback)
    {
        var vm = new DeletionDialogViewModel(text);

        Observable.Merge(vm.Yes.Select(_ => true), vm.No.Select(_ => false))
            .Take(1).Subscribe(
                isYes =>
                {
                    callback(isYes);
                    Content = AddictionsList;
                }
            );

        Content = vm;
    }

    void HandleFailureDialog(
        string addictionTitle,
        DateTime initialDateTime,
        string? initialNote,
        Action<Failure?> callback
    )
    {
        var addiction = AddictionsList.Addictions.Single(
            x => x.Title.Equals(addictionTitle)
        );
        var vm = new FailureDialogViewModel(
            new SortedSet<DateTime>(
                addiction.Failures.Select(x => x.FailedAt),
                new FailedAtComparer()
            ),
            initialDateTime,
            initialNote
        );

        Observable.Merge(vm.Ok, vm.Cancel.Select(_ => (Failure?)null)).Take(1)
            .Subscribe(
                failure =>
                {
                    callback(failure);
                    Content = AddictionsList;
                }
            );

        Content = vm;
    }

}
