using Microsoft.UI;
using Microsoft.UI.Xaml.Media;

using TvTime.ViewModels;

namespace TvTime;

public partial class App : Application
{
    public IThemeService ThemeService { get; set; }
    public static Window currentWindow = Window.Current;
    public string TvTimeVersion { get; set; }
    public IServiceProvider Services { get; }
    public new static App Current => (App) Application.Current;

    public static T GetService<T>()
        where T : class
    {
        if ((App.Current as App)!.Services.GetService(typeof(T)) is not T service)
        {
            throw new ArgumentException($"{typeof(T)} needs to be registered in ConfigureServices within App.xaml.cs.");
        }

        return service;
    }

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
        services.AddSingleton<IJsonNavigationViewService, JsonNavigationViewService>();

        services.AddTransient<MainViewModel>();
        services.AddTransient<BreadCrumbBarViewModel>();
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

        ThemeService = new ThemeService();
        ThemeService.Initialize(currentWindow);
        ThemeService.ConfigBackdrop(BackdropType.Mica);
        ThemeService.ConfigElementTheme(ElementTheme.Default);
        ThemeService.ConfigBackdropFallBackColorForWindow10(Current.Resources["ApplicationPageBackgroundThemeBrush"] as Brush);

        rootFrame.Navigate(typeof(MainPage));

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
