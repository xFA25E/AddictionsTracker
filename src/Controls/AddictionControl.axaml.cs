using System;
using AddictionsTracker.Models;
using Avalonia.Controls;

namespace AddictionsTracker.Controls;

public partial class AddictionControl : UserControl
{
    public AddictionControl() => InitializeComponent();
    public AddictionControl(
        Addiction addiction,
        Action<Addiction> insertFailure,
        Action<Addiction> update,
        Action<Addiction> delete,
        Action<Addiction> moveUp,
        Action<Addiction> moveDown
    ) : this()
    {
        button.Click += (_, _) => button.ContextMenu?.Open();
        addMI.Click += (_, _) => insertFailure(addiction);
        editMI.Click += (_, _) => update(addiction);
        removeMI.Click += (_, _) => delete(addiction);
        moveUpMI.Click += (_, _) => moveUp(addiction);
        moveDownMI.Click += (_, _) => moveDown(addiction);
        DataContext = addiction;
    }
}
