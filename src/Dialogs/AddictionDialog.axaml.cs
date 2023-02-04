using System;
using System.Collections.Generic;
using System.ComponentModel;
using Avalonia.Controls;

namespace AddictionsTracker.Dialogs;

public partial class AddictionDialog : Window
{
    public AddictionDialog() : this("Addiction", string.Empty, new string[0]) {}

    public AddictionDialog(
        string label,
        string initialInput,
        IEnumerable<string> forbiddenTitles
    )
    {
        InitializeComponent();
        DataContext = new AddictionDialogViewModel(forbiddenTitles)
        {
            Label = label,
            Title = initialInput,
            Ok = (title) => this.Close(title),
            Cancel = () => this.Close(null),
        };
    }
}

public class AddictionDialogViewModel : INotifyPropertyChanged
{
    HashSet<string> forbiddenTitles;

    public AddictionDialogViewModel(IEnumerable<string> forbiddenTitles)
    {
        this.forbiddenTitles = new(forbiddenTitles);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    string label = "Addiction";
    public string Label
    {
        get => label;
        set
        {
            label = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Label)));
        }
    }

    string title = "";
    public string Title
    {
        get => title;
        set
        {
            title = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Title)));
            CanOk = !(string.IsNullOrWhiteSpace(value)
                      || forbiddenTitles.Contains(value));
        }
    }

    void OkCommand() => Ok(Title);
    Action<string> ok = (_) => {};
    public Action<string> Ok {
        get => ok;
        set
        {
            ok = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Ok)));
        }
    }

    bool canOk = false;
    public bool CanOk
    {
        get => canOk;
        set
        {
            canOk = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanOk)));
        }
    }

    Action cancel = () => {};
    public Action Cancel {
        get => cancel;
        set
        {
            cancel = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Cancel)));
        }
    }
}
