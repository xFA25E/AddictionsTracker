using System;
using System.ComponentModel;
using Avalonia.Controls;

namespace AddictionsTracker.Dialogs;

public partial class ConfirmationDialog : Window
{

    public ConfirmationDialog() : this(string.Empty) {}

    public ConfirmationDialog(string prompt)
    {
        InitializeComponent();
        DataContext = new ConfirmationDialogViewModel()
        {
            Prompt = prompt,
            Yes = () => this.Close(true),
            No = () => this.Close(false),
        };
    }
}

public class ConfirmationDialogViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    string prompt = string.Empty;
    public string Prompt
    {
        get => prompt;
        set
        {
            prompt = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Prompt)));
        }
    }

    Action yes = () => {};
    public Action Yes {
        get => yes;
        set
        {
            yes = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Yes)));
        }
    }

    Action no = () => {};
    public Action No {
        get => no;
        set
        {
            no = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(No)));
        }
    }
}
