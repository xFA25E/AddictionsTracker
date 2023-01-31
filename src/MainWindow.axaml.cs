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

    private void zoom(int delta)
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
                        rect.SetValue(Canvas.LeftProperty, rect.GetValue(Canvas.LeftProperty) / dayWidth * newDayWidth);
                        rect.Width = newDayWidth;
                    }
                    else if (s is TextBlock label)
                    {
                        label.SetValue(Canvas.LeftProperty, label.GetValue(Canvas.LeftProperty) / dayWidth * newDayWidth);
                    }
                }
            }
            else if (c.Classes.Contains("failures"))
            {
                foreach (Rectangle f in ((StackPanel) ((Border) c).Child).Children)
                {
                    f.Width = f.Width / dayWidth * newDayWidth;
                }
            }
        }
        aGrid.EndBatchUpdate();

        dayWidth = newDayWidth;
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
                    var left = dateNow.Subtract(date).TotalDays * dayWidth - dayWidth;

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
        addictions.Each(1, (addiction, row) =>
        {
            aGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));

            // ADDICTION
            {
                var border = new Border();
                border.BorderBrush = new SolidColorBrush(Color.Parse("Black"));
                border.BorderThickness = new Avalonia.Thickness(0, 0, 1, 1);
                border.SetValue(Grid.RowProperty, row);
                aGrid.Children.Add(border);

                var addictionLabel = new TextBlock();
                addictionLabel.Text = addiction.Title;
                addictionLabel.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
                addictionLabel.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right;
                addictionLabel.Padding = new Avalonia.Thickness(8);
                border.Child = addictionLabel;
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

                var previousFailureDate = dateNow;
                foreach (var failure in addiction.Failures)
                {
                    var abstinencePeriod =
                        previousFailureDate.Subtract(failure.FailedAt);
                    previousFailureDate = failure.FailedAt;

                    var abstinenceArea = new Rectangle();
                    abstinenceArea.Width = abstinencePeriod.TotalDays * dayWidth - dayWidth;
                    abstinenceArea.Fill = new SolidColorBrush(Color.Parse("Green"));
                    failures.Children.Add(abstinenceArea);

                    var failureArea = new Rectangle();
                    failureArea.Width = dayWidth;
                    failureArea.Fill = new SolidColorBrush(Color.Parse("Red"));
                    failures.Children.Add(failureArea);
                }
            }
        }
        );

        aGrid.EndBatchUpdate();

        Console.WriteLine("{0}", this.Bounds);
    }
}

public static class Extensions
{
    static readonly TimeOnly zeroTime = new TimeOnly(0, 0, 0);

    public static TimeSpan Subtract(this DateOnly a, DateOnly b)
    {
        return a.ToDateTime(zeroTime) - b.ToDateTime(zeroTime);

    }

    public static void Each<T>(this IEnumerable<T> ie, int start, Action<T, int> action)
    {
        var i = start;
        foreach (var e in ie) action(e, i++);
    }
}
