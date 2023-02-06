using Avalonia.Controls;

namespace AddictionsTracker.Dialogs;

public partial class ConfirmationDialog : Window
{

    public ConfirmationDialog() => InitializeComponent();
    public ConfirmationDialog(string prompt) : this()
    {
        this.prompt.Text = prompt;
        yes.Click += (_, _) => this.Close(true);
        no.Click += (_, _) => this.Close(false);
    }
}
