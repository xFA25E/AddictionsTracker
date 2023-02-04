using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace AddictionsTracker.Dialogs;

public partial class FailureDialog : Window
{
    DateOnly now = DateTime.Now.ToDateOnly();

    HashSet<DateOnly> forbiddenDates;

    public FailureDialog()
        : this(
            "ADDICTION",
            DateOnly.FromDateTime(DateTime.Now),
            string.Empty,
            new DateOnly[0]
        ) {}

    public FailureDialog(
        string addictionTitle,
        DateOnly initialDate,
        string initialNote,
        IEnumerable<DateOnly> forbiddenDates
    )
    {
        this.forbiddenDates = new(forbiddenDates);

        InitializeComponent();
        ((TextBlock)textBlock).Text = $"Failed {addictionTitle} at";
        ((TextBox)textBox).Text = initialNote;
        ((DatePicker)datePicker).SelectedDate = initialDate.ToDateTimeOffset();
    }

    public void datePicker_SelectedDateChanged(
        object? sender,
        DatePickerSelectedValueChangedEventArgs args
    )
    {
        var newValue = args.NewDate.Value.ToDateOnly();
        ((Button)okButton).IsEnabled =
            now >= newValue && !forbiddenDates.Contains(newValue);
    }

    public void okButton_Click(object? sender, RoutedEventArgs args)
    {
        this.Close(
            (
                ((DatePicker)datePicker).SelectedDate.Value.ToDateOnly(),
                ((TextBox)textBox).Text
            )
        );
    }

    public void cancelButton_Click(object? sender, RoutedEventArgs args)
    {
        this.Close(null);
    }
}
