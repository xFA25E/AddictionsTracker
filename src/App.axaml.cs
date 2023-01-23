using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using AddictionsTracker.ViewModels;
using AddictionsTracker.Views;
using AddictionsTracker.Services;

namespace AddictionsTracker;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        base.OnFrameworkInitializationCompleted();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var database = new Database();

            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(database),
            };
        }
    }
}
