using Avalonia.Controls;
using AddictionsTracker.Controls;
using AddictionsTracker.Dialogs;
using AddictionsTracker.Services;
using System;
using AddictionsTracker.Models;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Input;
using System.Linq;
using Avalonia.Interactivity;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections.Generic;

namespace AddictionsTracker;

public partial class MainWindow : Window
{
    Database database = new Database();
    ObservableCollection<Addiction> addictions = new();

    public MainWindow()
    {
        addictions.CollectionChanged += addictionsChangedHandler;

        InitializeComponent();
        button.Click += (_, _) => insertAddiction();
        grid.PointerWheelChanged += (_, a) =>
        {
            if (a.KeyModifiers == KeyModifiers.Control)
                DayWidth.Instance.Width += (int)a.Delta.Y;
        };

        database.PopulateAddictions(addictions);
    }

    private void addictionsChangedHandler(
        object? sender,
        NotifyCollectionChangedEventArgs args
    )
    {
        if (args.Action == NotifyCollectionChangedAction.Add
            && args.NewItems != null && args.NewItems.Count != 0
            && args.NewItems[0] is Addiction addiction)
        {
            var addedRow = args.NewStartingIndex + 1;
            var addictionControl = new AddictionControl(
                addiction,
                insertFailure,
                updateAddiction,
                deleteAddiction,
                moveUpAddiction,
                moveDownAddiction
            );
            var failuresControl = new FailuresControl(
                addiction,
                updateFailure,
                deleteFailure
            );

            Grid.SetRow(addictionControl, addedRow);
            Grid.SetRow(failuresControl, addedRow);
            Grid.SetColumn(failuresControl, 1);

            grid.BeginBatchUpdate();
            grid.RowDefinitions.Insert(addedRow, new RowDefinition(GridLength.Auto));
            foreach (Control c in grid.Children)
            {
                var row = Grid.GetRow(c);
                if (addedRow <= row)
                    Grid.SetRow(c, row + 1);
            }
            grid.Children.Add(addictionControl);
            grid.Children.Add(failuresControl);
            grid.EndBatchUpdate();
        }
        else if (args.Action == NotifyCollectionChangedAction.Remove)
        {
            var removedRow = args.OldStartingIndex + 1;
            var controlsToRemove = new LinkedList<Control>();

            grid.BeginBatchUpdate();
            foreach (Control c in grid.Children)
            {
                var row = Grid.GetRow(c);
                if (removedRow == row)
                    controlsToRemove.AddLast(c);
                else if (removedRow < row)
                    Grid.SetRow(c, row - 1);
            }
            grid.Children.RemoveAll(controlsToRemove);
            grid.RowDefinitions.RemoveAt(removedRow);
            grid.EndBatchUpdate();
        }
        else if (args.Action == NotifyCollectionChangedAction.Move)
        {
            var rowA = args.NewStartingIndex + 1;
            var rowB = args.OldStartingIndex + 1;

            grid.BeginBatchUpdate();
            foreach (Control c in grid.Children)
            {
                var row = Grid.GetRow(c);
                if (row == rowA)
                    Grid.SetRow(c, rowB);
                else if (row == rowB)
                    Grid.SetRow(c, rowA);
            }
            grid.RowDefinitions.Move(rowA, rowB);
            grid.EndBatchUpdate();
        }
    }

    public async void insertAddiction()
    {
        var dialog = new AddictionDialog(
            "Add Addiction",
            string.Empty,
            addictions.Select(a => a.Title)
        );

        var result = await dialog.ShowDialog<string?>(this);
        if (result != null)
            addictions.Add(database.InsertAddiction(result));
    }

    private async void updateAddiction(Addiction addiction)
    {
        var dialog = new AddictionDialog(
            "Edit Addiction",
            addiction.Title,
            addictions
            .Select(a => a.Title)
            .Where(t => !t.Equals(addiction.Title))
        );

        var result = await dialog.ShowDialog<string?>(this);
        if (result != null && !result.Equals(addiction.Title))
        {
            database.UpdateAddiction(addiction, result);
            addiction.Title = result;
        }
    }

    private async void deleteAddiction(Addiction addiction)
    {
        var dialog = new ConfirmationDialog(
            $"Are you sure that you want to delete {addiction.Title}?"
        );
        if (await dialog.ShowDialog<bool>(this))
        {
            database.DeleteAddiction(addiction);
            addictions.Remove(addiction);
        }
    }

    private void moveUpAddiction(Addiction addiction)
    {
        var row = addictions.IndexOf(addiction);
        if (0 < row)
            addictions.Move(row, row - 1);
    }

    private void moveDownAddiction(Addiction addiction)
    {
        var row = addictions.IndexOf(addiction);
        if (row < addictions.Count - 1)
            addictions.Move(row, row + 1);
    }

    private async void insertFailure(Addiction addiction)
    {
        var dialog = new FailureDialog(
            addiction.Title,
            DateTime.Now.ToDateOnly(),
            string.Empty,
            addiction.Failures.Select(f => f.FailedAt)
        );
        var result = await dialog.ShowDialog<(DateOnly FailedAt, string Note)?>(this);
        if (result != null)
        {
            var (failedAt, note) = result.Value;
            var failure = database.InsertFailure(addiction, failedAt, note);
            addiction.InsertFailure(failure);
        }
    }

    private async void updateFailure(Addiction addiction, Failure failure)
    {
        var dialog = new FailureDialog(
            addiction.Title,
            failure.FailedAt,
            failure.Note,
            addiction.Failures
            .Select(f => f.FailedAt)
            .Where(d => !d.Equals(failure.FailedAt))
        );
        var result = await dialog.ShowDialog<(DateOnly FailedAt, string Note)?>(this);
        if (result != null)
        {
            var (failedAt, note) = result.Value;
            if (!(failedAt.Equals(failure.FailedAt)
                  && note.Equals(failure.Note)))
            {
                database.UpdateFailure(failure, failedAt, note);
                failure.FailedAt = failedAt;
                failure.Note = note;
            }
        }
    }

    private async void deleteFailure(Addiction addiction, Failure failure)
    {
        var dialog = new ConfirmationDialog(
            $"Are you sure that you want to delete failure of {addiction.Title} at {failure.FailedAt.ToString("yyyy MMMM dd")}?"
        );
        if (await dialog.ShowDialog<bool>(this))
        {
            database.DeleteFailure(failure);
            addiction.DeleteFailure(failure);
        }
    }

    public void CreateControls()
    {
        var dateNow = DateTime.Now.ToDateOnly();

        grid.Children.Clear();
        grid.RowDefinitions.Clear();

        grid.BeginBatchUpdate();

        grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));

        // SCALE
        {
            var border = new Border();
            border.BorderBrush = new SolidColorBrush(Color.Parse("Black"));
            border.BorderThickness = new Avalonia.Thickness(0, 0, 0, 1);
            border.Classes.Add("scale");
            border.SetValue(Grid.ColumnProperty, 1);
            grid.Children.Add(border);

            var scale = new Canvas();
            border.Child = scale;

            var minDate = addictions.Select(a => a.Failures.LastOrDefault()?.FailedAt).Min();
            if (minDate != null)
            {
                for (DateOnly date = new DateOnly(dateNow.Year, dateNow.Month, 1);
                     date > minDate;
                     date = date.AddMonths(-1))
                {
                    var left = dateNow.Subtract(date) * DayWidth.Instance.Width;

                    var label = new TextBlock();
                    label.Text = date.ToString("MM/yy");
                    label.FontFamily = new FontFamily("monospace");
                    label.SetValue(Canvas.TopProperty, 0);
                    label.SetValue(Canvas.LeftProperty, left);
                    scale.Children.Add(label);

                    var rect = new Rectangle();
                    rect.SetValue(Canvas.TopProperty, 20);
                    rect.SetValue(Canvas.LeftProperty, left);
                    rect.Width = DayWidth.Instance.Width;
                    rect.Height = 10;
                    rect.Fill = new SolidColorBrush(Color.Parse("Black"));
                    scale.Children.Add(rect);
                }
            }
        }
        grid.EndBatchUpdate();
    }
}
