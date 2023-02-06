using System;
using System.ComponentModel;
using AddictionsTracker.Models;
using Avalonia.Controls;
using Avalonia.Interactivity;

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
        DataContext = new AddictionControlViewModel(addiction)
        {
            InsertFailure = insertFailure,
            Update = update,
            Delete = delete,
            MoveUp = moveUp,
            MoveDown = moveDown
        };
    }

    void border_Tapped(object? sender, RoutedEventArgs args)
    {
        if (sender is Border border) border.ContextMenu?.Open();
    }
}

public class AddictionControlViewModel : INotifyPropertyChanged
{
    Addiction addiction;

    public event PropertyChangedEventHandler? PropertyChanged;

    public AddictionControlViewModel(Addiction addiction)
    {
        this.addiction = addiction;
        this.addiction.PropertyChanged += (s, a) =>
        {
            if (a.PropertyName?.Equals(nameof(addiction.Title)) != null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Title)));
            }
        };
    }

    public string Title => addiction.Title;

    public void InsertFailureCommand() => InsertFailure(addiction);
    Action<Addiction> insertFailure = (_) => { };
    public Action<Addiction> InsertFailure
    {
        get => insertFailure;
        set
        {
            insertFailure = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InsertFailure)));
        }
    }

    public void UpdateCommand() => Update(addiction);
    Action<Addiction> update = (_) => { };
    public Action<Addiction> Update
    {
        get => update;
        set
        {
            update = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Update)));
        }
    }

    public void DeleteCommand() => Delete(addiction);
    Action<Addiction> delete = (_) => { };
    public Action<Addiction> Delete
    {
        get => delete;
        set
        {
            delete = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Delete)));
        }
    }

    public void MoveUpCommand() => MoveUp(addiction);
    Action<Addiction> moveUp = (_) => { };
    public Action<Addiction> MoveUp
    {
        get => moveUp;
        set
        {
            moveUp = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MoveUp)));
        }
    }

    public void MoveDownCommand() => MoveDown(addiction);
    Action<Addiction> moveDown = (_) => { };
    public Action<Addiction> MoveDown
    {
        get => moveDown;
        set
        {
            moveDown = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MoveDown)));
        }
    }
}
