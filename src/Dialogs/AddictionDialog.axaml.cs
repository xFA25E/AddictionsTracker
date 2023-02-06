using System.Collections.Generic;
using Avalonia.Controls;

namespace AddictionsTracker.Dialogs;

public partial class AddictionDialog : Window
{
    public AddictionDialog() => InitializeComponent();
    public AddictionDialog(
        string label,
        string initialInput,
        IEnumerable<string> forbiddenTitles
    ) : this()
    {
        HashSet<string> titles = new(forbiddenTitles);
        textBox.PropertyChanged += (_, a) =>
        {
            if (a.Property.Equals(TextBox.TextProperty)
                && a.NewValue is string title)
                ok.IsEnabled = !(title.Equals("") || titles.Contains(title));
        };

        ok.Click += (_, _) => this.Close(textBox.Text);
        cancel.Click += (_, _) => this.Close(null);

        textBlock.Text = label;
        textBox.Text = initialInput;
    }
}
