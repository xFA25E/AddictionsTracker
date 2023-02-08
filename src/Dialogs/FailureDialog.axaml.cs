using System;
using System.Collections.Generic;
using Avalonia.Controls;

namespace AddictionsTracker.Dialogs;

public partial class FailureDialog : Window
{
    DateOnly date;

    public FailureDialog() => InitializeComponent();
    public FailureDialog(
        string addictionTitle,
        DateOnly initialDate,
        string initialNote,
        IEnumerable<DateOnly> forbiddenDates
    ) : this()
    {
        HashSet<DateOnly> dates = new(forbiddenDates);
        datePicker.SelectedDateChanged += (_, a) =>
        {
            if (a.NewDate is DateTimeOffset dto)
            {
                date = dto.ToDateOnly();
                ok.IsEnabled = Globals.Now >= date && !dates.Contains(date);
            }
        };

        ok.Click += (_, _) => this.Close((date, textBox.Text));
        cancel.Click += (_, _) => this.Close(null);

        date = initialDate;
        datePicker.SelectedDate = date.ToDateTimeOffset();
        textBox.Text = initialNote;
        textBlock.Text = $"Failed {addictionTitle} at";
    }
}
