using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace AddictionsTracker.Dialogs;

public partial class AddictionDialog : Window
{
    HashSet<string> forbiddenTitles;

    public string Label
    {
        get => ((TextBlock)label).Text;
        set => ((TextBlock)label).Text = value;
    }

    public AddictionDialog() : this("Addiction", string.Empty, new string[0]) {}

    public AddictionDialog(string initialInput)
        : this("Addiction", initialInput, new string[0]) {}

    public AddictionDialog(
        string label,
        string initialInput,
        IEnumerable<string> forbiddenTitles
    )
    {
        InitializeComponent();
        this.forbiddenTitles = new(forbiddenTitles);
        Label = label;
        ((TextBox)textBox).Text = initialInput;
    }

    public void okButton_Click(object? sender, RoutedEventArgs args)
    {
        this.Close(((TextBox)textBox).Text);
    }

    public void cancelButton_Click(object? sender, RoutedEventArgs args)
    {
        this.Close(null);
    }

    private void textBox_TextPropertyChanged(
        object? sender,
        AvaloniaPropertyChangedEventArgs args
    )
    {
        if (args.Property == TextBox.TextProperty)
        {
            var newValue = (string?)args.NewValue;
            ((Button)okButton).IsEnabled =
                !string.IsNullOrWhiteSpace(newValue)
                && !forbiddenTitles.Contains(newValue);
        }
    }
}
