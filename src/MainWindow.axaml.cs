using Avalonia.Controls;
using AddictionsTracker.Services;
using AddictionsTracker.Dialogs;
using Avalonia.Controls.Primitives;
using System;
using System.Collections.Generic;
using AddictionsTracker.Models;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Input;
using System.Linq;
using Avalonia.Interactivity;

namespace AddictionsTracker;

public partial class MainWindow : Window
{
    Database database = new Database();
    int dayWidth = 2;

    List<Addiction> addictions;

    public MainWindow()
    {
        addictions = database.GetAddictions();
        InitializeComponent();
        CreateControls();
    }

    public void addictionsGrid_PointerWheelChanged(object? sender, PointerWheelEventArgs args)
    {
        if (args.KeyModifiers == KeyModifiers.Control)
        {
            zoom((int)args.Delta.Y);
        }
    }

    void zoom(int delta)
    {
        Grid aGrid = addictionsGrid;
        int newDayWidth = Math.Max(dayWidth + delta, 2);
        if (dayWidth == newDayWidth) return;

        aGrid.BeginBatchUpdate();
        foreach (var c in aGrid.Children)
        {
            if (c.Classes.Contains("scale"))
            {
                foreach (var s in ((Canvas) ((Border) c).Child).Children)
                {
                    if (s is Rectangle rect)
                    {
                        rect.SetValue(Canvas.LeftProperty, rect.GetValue(Canvas.LeftProperty) / (dayWidth + 1) * (newDayWidth + 1));
                        rect.Width = newDayWidth;
                    }
                    else if (s is TextBlock label)
                    {
                        label.SetValue(Canvas.LeftProperty, label.GetValue(Canvas.LeftProperty) / (dayWidth + 1) * (newDayWidth + 1));
                    }
                }
            }
            else if (c.Classes.Contains("failures"))
            {
                foreach (var f in ((StackPanel) ((Border) c).Child).Children)
                {
                    if (f is Rectangle rect)
                    {
                        rect.Width = rect.Width / (dayWidth + 1) * (newDayWidth + 1);
                    }
                    else if (f is Border b)
                    {
                        ((Rectangle) b.Child).Width = ((Rectangle) b.Child).Width / dayWidth * newDayWidth;
                    }
                }
            }
        }
        aGrid.EndBatchUpdate();

        dayWidth = newDayWidth;
    }

    private void swapRows(int rowA, int rowB)
    {
        Grid aGrid = addictionsGrid;

        aGrid.BeginBatchUpdate();
        foreach (Control c in aGrid.Children)
        {
            if (Grid.GetRow(c) == rowA)
            {
                Grid.SetRow(c, rowB);
            }
            else if (Grid.GetRow(c) == rowB)
            {
                Grid.SetRow(c, rowA);
            }
        }
        aGrid.EndBatchUpdate();

        var temp = addictions[rowA - 1];
        addictions[rowA - 1] = addictions[rowB - 1];
        addictions[rowB - 1] = temp;
    }

    private void moveUp(int row)
    {
        if (row > 1) swapRows(row, row - 1);
    }

    private void moveDown(int row)
    {
        if (row < addictions.Count) swapRows(row, row + 1);
    }

    private async void addAddiction(object? sender, RoutedEventArgs args)
    {
        var dialog = new AddictionDialog(
            "Add Addiction",
            string.Empty,
            addictions.Select(a => a.Title)
        );

        var result = await dialog.ShowDialog<string?>(this);
        if (result != null)
        {
            var addiction = database.InsertAddiction(result);
            addictions.Add(addiction);
            CreateControls();
        }
    }

    private async void editAddition(Addiction addiction, TextBlock label)
    {
        var dialog = new AddictionDialog(
            "Edit Addiction",
            addiction.Title,
            addictions
            .Select(a => a.Title)
            .Where(t => !t.Equals(addiction.Title))
        );

        var result = await dialog.ShowDialog<string?>(this);
        if (result != null)
        {
            database.UpdateAddiction(addiction, result);
            addiction.Title = result;
            label.Text = result;
        }
    }

    private async void removeAddiction(int index)
    {
        var dialog = new ConfirmationDialog(
            $"Are you sure that you want to delete {addictions[index].Title}?"
        );
        if (await dialog.ShowDialog<bool>(this))
        {
            database.DeleteAddiction(addictions[index]);
            addictions.RemoveAt(index);
            CreateControls();
        }
    }

    private async void addFailure(Addiction addiction)
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
            addiction.Failures.Add(failure);
            CreateControls();
        }
    }

    private async void editFailure(Addiction addiction, Failure failure)
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
            database.UpdateFailure(failure, failedAt, note);
            addiction.Failures.Remove(failure);
            failure.FailedAt = failedAt;
            failure.Note = note;
            addiction.Failures.Add(failure);
            CreateControls();
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
            addiction.Failures.Remove(failure);
            CreateControls();
        }
    }

    public void CreateControls()
    {
        Grid aGrid = addictionsGrid;
        var dateNow = DateTime.Now.ToDateOnly();

        aGrid.Children.Clear();
        aGrid.RowDefinitions.Clear();

        aGrid.BeginBatchUpdate();

        aGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));

        // ADD ADDICTION
        {
            var border = new Border();
            border.BorderBrush = new SolidColorBrush(Color.Parse("Black"));
            border.BorderThickness = new Avalonia.Thickness(0, 0, 1, 1);
            aGrid.Children.Add(border);

            var addAddictionButton = new Button();
            addAddictionButton.Content = "Add Addiction";
            addAddictionButton.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
            addAddictionButton.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch;
            addAddictionButton.HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center;
            addAddictionButton.Click += addAddiction;
            border.Child = addAddictionButton;
        }

        // SCALE
        {
            var border = new Border();
            border.BorderBrush = new SolidColorBrush(Color.Parse("Black"));
            border.BorderThickness = new Avalonia.Thickness(0, 0, 0, 1);
            border.Classes.Add("scale");
            border.SetValue(Grid.ColumnProperty, 1);
            aGrid.Children.Add(border);

            var scale = new Canvas();
            border.Child = scale;

            var minDate = addictions.Select(a => a.Failures.Max?.FailedAt).Min();
            if (minDate != null)
            {
                for (DateOnly date = new DateOnly(dateNow.Year, dateNow.Month, 1);
                     date > minDate;
                     date = date.AddMonths(-1))
                {
                    var left = dateNow.Subtract(date).TotalDays * (dayWidth + 1);

                    var label = new TextBlock();
                    label.Text = date.ToString("MM/yy");
                    label.FontFamily = new FontFamily("monospace");
                    label.SetValue(Canvas.TopProperty, 0);
                    label.SetValue(Canvas.LeftProperty, left);
                    scale.Children.Add(label);

                    var rect = new Rectangle();
                    rect.SetValue(Canvas.TopProperty, 20);
                    rect.SetValue(Canvas.LeftProperty, left);
                    rect.Width = dayWidth;
                    rect.Height = 10;
                    rect.Fill = new SolidColorBrush(Color.Parse("Black"));
                    scale.Children.Add(rect);
                }
            }
        }

        // ADDICTIONS AND FAILURES

        int row = 0;
        foreach (var addiction in addictions)
        {
            row++;
            aGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));

            // ADDICTION
            {
                var border = new Border();
                border.Background = new SolidColorBrush(Color.Parse("White"));
                border.BorderBrush = new SolidColorBrush(Color.Parse("Black"));
                border.BorderThickness = new Avalonia.Thickness(0, 0, 1, 1);
                border.SetValue(Grid.RowProperty, row);
                aGrid.Children.Add(border);

                var addictionLabel = new TextBlock();
                addictionLabel.Text = addiction.Title;
                addictionLabel.Padding = new Avalonia.Thickness(8);
                border.Child = addictionLabel;

                // Context Menu
                {
                    var addFailureMI = new MenuItem();
                    addFailureMI.Header = $"Add Failure to {addiction.Title}";
                    addFailureMI.Click += (s, a) => addFailure(addiction);

                    var editAddictionMI = new MenuItem();
                    editAddictionMI.Header = $"Edit {addiction.Title}";
                    editAddictionMI.Click += (s, a) => editAddition(
                        addiction,
                        addictionLabel
                    );

                    var deleteAddictionMI = new MenuItem();
                    deleteAddictionMI.Header = $"Delete {addiction.Title}";
                    deleteAddictionMI.Click += (s, a) => removeAddiction(
                        Grid.GetRow(border) - 1
                    );

                    var moveUpMI = new MenuItem();
                    moveUpMI.Header = "Move Up";
                    moveUpMI.Click += (s, a) => moveUp(Grid.GetRow(border));

                    var moveDownMI = new MenuItem();
                    moveDownMI.Header = "Move Down";
                    moveDownMI.Click += (s, a) => moveDown(Grid.GetRow(border));

                    border.ContextMenu = new ContextMenu()
                    {
                        Items = new Control[]
                        {
                            addFailureMI,
                            editAddictionMI,
                            deleteAddictionMI,
                            moveUpMI,
                            moveDownMI,
                            new MenuItem() { Header = "Close Menu" }
                        },
                    };
                    border.Tapped += (s, a) => border.ContextMenu.Open();
                }
            }

            // FAILURES
            {
                var border = new Border();
                border.BorderBrush = new SolidColorBrush(Color.Parse("Black"));
                border.BorderThickness = new Avalonia.Thickness(0, 0, 0, 1);
                border.Classes.Add("failures");
                border.SetValue(Grid.RowProperty, row);
                border.SetValue(Grid.ColumnProperty, 1);
                aGrid.Children.Add(border);

                var failures = new StackPanel();
                failures.Orientation = Avalonia.Layout.Orientation.Horizontal;
                border.Child = failures;

                var previousFailureDate = dateNow.AddDays(1);
                foreach (var failure in addiction.Failures)
                {
                    var abstinenceDays = previousFailureDate
                        .Subtract(failure.FailedAt)
                        .TotalDays - 1;
                    previousFailureDate = failure.FailedAt;

                    if (abstinenceDays != 0)
                    {
                        var abstinenceArea = new Rectangle();
                        abstinenceArea.Width = abstinenceDays * (dayWidth + 1);
                        abstinenceArea.Fill = new SolidColorBrush(Color.Parse("Green"));
                        abstinenceArea.SetValue(ToolTip.TipProperty, $"Abstained for {abstinenceDays} day{(abstinenceDays == 1 ? "" : "s")} from {addiction.Title}");
                        abstinenceArea.SetValue(ToolTip.ShowDelayProperty, 15);
                        failures.Children.Add(abstinenceArea);
                    }

                    var failureBorder = new Border();
                    failureBorder.BorderBrush = new SolidColorBrush(Color.Parse("Black"));
                    failureBorder.BorderThickness = new Avalonia.Thickness(1, 0, 0, 0);
                    failureBorder.SetValue(
                        ToolTip.TipProperty,
                        $@"At {failure.FailedAt.ToString("yyyy-MM-dd")} failed {addiction.Title}
{failure.Note}".Trim()
);
                    failureBorder.SetValue(ToolTip.ShowDelayProperty, 15);
                    failures.Children.Add(failureBorder);

                    var failureArea = new Rectangle();
                    failureArea.Width = dayWidth;
                    failureArea.Fill = new SolidColorBrush(Color.Parse("Red"));
                    failureBorder.Child = failureArea;

                    // Context Menu
                    {
                        var editFailureMI = new MenuItem();
                        editFailureMI.Header = $"Edit";
                        editFailureMI.Click += (s, a) => editFailure(
                            addiction,
                            failure
                        );

                        var deleteFailureMI = new MenuItem();
                        deleteFailureMI.Header = $"Delete";
                        deleteFailureMI.Click += (s, a) => deleteFailure(
                            addiction,
                            failure
                        );

                        failureBorder.ContextMenu = new ContextMenu()
                        {
                            Items = new Control[]
                            {
                                editFailureMI,
                                deleteFailureMI,
                                new MenuItem() { Header = "Close Menu" }
                            },
                        };
                        failureBorder.Tapped += (s, a) => failureBorder.ContextMenu.Open();
                    }
                }
            }
        }

        aGrid.EndBatchUpdate();
    }
}

public static class Extensions
{
    static readonly TimeOnly zeroTime = new TimeOnly(0, 0, 0);

    public static TimeSpan Subtract(this DateOnly a, DateOnly b)
    {
        return a.ToDateTime() - b.ToDateTime();

    }

    public static DateTime ToDateTime(this DateOnly d)
    {
        return d.ToDateTime(zeroTime);
    }

    public static DateTimeOffset ToDateTimeOffset(this DateOnly d)
    {
        return new DateTimeOffset(d.ToDateTime());
    }

    public static DateOnly ToDateOnly(this DateTime dt)
    {
        return DateOnly.FromDateTime(dt);
    }

    public static DateOnly ToDateOnly(this DateTimeOffset dto)
    {
        return dto.DateTime.ToDateOnly();
    }
}
