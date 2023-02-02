using Avalonia.Controls;
using AddictionsTracker.Services;
using Avalonia.Controls.Primitives;
using System;
using System.Collections.Generic;
using AddictionsTracker.Models;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Input;
using System.Linq;

namespace AddictionsTracker;

public partial class MainWindow : Window
{
    Database database = new Database();
    int dayWidth = 2;

    List<Addiction> addictions;
    List<(Border, Border)> addictionPositions = new();

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

        var temp = addictions[rowA];
        addictions[rowA] = addictions[rowB];
        addictions[rowB] = temp;
    }

    private void moveUp(int row)
    {
        if (row > 1)
        {
            swapRows(row, row - 1);
        }
    }

    private void moveDown(int row)
    {
        if (row < addictions.Count)
        {
            swapRows(row, row + 1);
        }
    }

    public void CreateControls()
    {
        Grid aGrid = addictionsGrid;
        var dateNow = DateOnly.FromDateTime(DateTime.Now);

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
                border.BorderBrush = new SolidColorBrush(Color.Parse("Black"));
                border.BorderThickness = new Avalonia.Thickness(0, 0, 1, 1);
                border.SetValue(Grid.RowProperty, row);
                aGrid.Children.Add(border);

                var dockPanel = new DockPanel();
                border.Child = dockPanel;

                var addictionLabel = new TextBlock();
                addictionLabel.Text = addiction.Title;
                addictionLabel.SetValue(DockPanel.DockProperty, Dock.Right);
                addictionLabel.Padding = new Avalonia.Thickness(8);
                dockPanel.Children.Add(addictionLabel);

                var editButton = new Button();
                editButton.Content = "*";
                editButton.SetValue(DockPanel.DockProperty, Dock.Left);
                dockPanel.Children.Add(editButton);

                // Context Menu
                {
                    var addFailureMI = new MenuItem();
                    addFailureMI.Header = $"Add Failure to {addiction.Title}";

                    var editAddictionMI = new MenuItem();
                    editAddictionMI.Header = $"Edit {addiction.Title}";

                    var deleteAddictionMI = new MenuItem();
                    deleteAddictionMI.Header = $"Delete {addiction.Title}";

                    var moveUpMI = new MenuItem();
                    moveUpMI.Header = "Move Up";
                    moveUpMI.Click += (s, a) => moveUp(Grid.GetRow(border));

                    var moveDownMI = new MenuItem();
                    moveDownMI.Header = "Move Down";
                    moveDownMI.Click += (s, a) => moveDown(Grid.GetRow(border));

                    editButton.ContextMenu = new ContextMenu()
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
                    editButton.Click += (s, a) => editButton.ContextMenu.Open();
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

                // TODAY
                {
                    var todayArea = new Rectangle();
                    todayArea.Width = dayWidth + 1;
                    failures.Children.Add(todayArea);

                    var latestFailure = addiction.Failures.Min?.FailedAt;
                    if (latestFailure != null && latestFailure.Equals(dateNow))
                    {
                        todayArea.Fill = new SolidColorBrush(Color.Parse("Red"));
                    }
                    else
                    {
                        todayArea.Fill = new SolidColorBrush(Color.Parse("Green"));
                    }
                }

                var previousFailureDate = dateNow;
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
                    failureBorder.SetValue(ToolTip.TipProperty, $"At {failure.FailedAt.ToString("yyyy-MM-dd")} failed {addiction.Title}");
                    failureBorder.SetValue(ToolTip.ShowDelayProperty, 15);
                    failures.Children.Add(failureBorder);

                    var failureArea = new Rectangle();
                    failureArea.Width = dayWidth;
                    failureArea.Fill = new SolidColorBrush(Color.Parse("Red"));
                    failureBorder.Child = failureArea;
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
}
