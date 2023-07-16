using Microsoft.UI;
using Microsoft.UI.Xaml.Media;

using TvTime.ViewModels;

namespace TvTime;

public partial class App : Application
{
    public static Window currentWindow = Window.Current;
    public NavigationManager NavigationManager { get; set; }
    public ThemeManager ThemeManager { get; set; }
    public string TvTimeVersion { get; set; }
    public IServiceProvider Services { get; }
    public new static App Current => (App) Application.Current;

    public App()
    {
        Services = ConfigureServices();

        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

        this.InitializeComponent();
        TvTimeVersion = VersionHelper.GetVersion();

        CreateDirectory();
    }

    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddTransient<HomeLandingsViewModel>();
        services.AddTransient<SettingsViewModel>();
        services.AddTransient<ServerViewModel>();
        services.AddTransient<MediaViewModel>();
        services.AddTransient<DetailsViewModel>();
        services.AddTransient<IMDBDetailsViewModel>();
        services.AddTransient<SubsceneViewModel>();
        services.AddTransient<SubsceneDetailViewModel>();

        //Settings
        services.AddTransient<AppUpdateSettingViewModel>();
        services.AddTransient<AboutUsSettingViewModel>();
        services.AddTransient<BackupSettingViewModel>();
        services.AddTransient<ThemeSettingViewModel>();
        services.AddTransient<SubtitleSettingViewModel>();
        services.AddTransient<GeneralSettingViewModel>();
        services.AddTransient<HeaderStyleSettingViewModel>();
        services.AddTransient<DescriptionStyleSettingViewModel>();

        return services.BuildServiceProvider();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        currentWindow = new Window();
        currentWindow.AppWindow.TitleBar.ExtendsContentIntoTitleBar = true;
        currentWindow.AppWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;
        if (currentWindow.Content is not Frame rootFrame)
        {
            currentWindow.Content = rootFrame = new Frame();
        }

        rootFrame.Navigate(typeof(MainPage));

        ThemeManager = ThemeManager.Initialize(currentWindow, new ThemeOptions
        {
            BackdropFallBackColorForWindows10 = Current.Resources["ApplicationPageBackgroundThemeBrush"] as Brush
        });

        if (Settings.SubtitleLanguagesCollection == null || Settings.SubtitleLanguagesCollection.Count == 0)
        {
            Settings.SubtitleLanguagesCollection = SubtitleLanguageCollection();
        }

        currentWindow.Title = currentWindow.AppWindow.Title = $"TvTime v{TvTimeVersion}";
        currentWindow.AppWindow.SetIcon("Assets/Fluent/icon.ico");

        currentWindow.Activate();

        UnhandledException += App_UnhandledException;
    }

    private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        Log(e.Exception.Message);
    }

    public static void Log(string message)
    {
        try
        {
            using (StreamWriter writer = File.AppendText("Log.txt"))
            {
                string logEntry = $"{DateTime.Now}{Environment.NewLine}{message}{Environment.NewLine}-----{Environment.NewLine}";
                writer.WriteLine(logEntry);
            }
        }
        catch (Exception ex)
        {
            // Handle any exceptions that occur during logging
            Console.WriteLine($"Error writing to log file: {ex.Message}");
        }
    }
}
