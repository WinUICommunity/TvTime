using System.Net;

using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter;

namespace TvTime;
public partial class App : Application
{
    public static Window currentWindow = Window.Current;
    public IServiceProvider Services { get; }
    public new static App Current => (App)Application.Current;
    public string AppVersion { get; set; } = ApplicationHelper.GetAppVersion();
    public string AppName { get; set; } = "TvTime";

    public static T GetService<T>() where T : class
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
    }

    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IThemeService, ThemeService>();
        services.AddSingleton<IJsonNavigationViewService>(factory =>
        {
            var json = new JsonNavigationViewService();
            json.ConfigDefaultPage(typeof(HomeLandingPage));
            json.ConfigSettingsPage(typeof(SettingsPage));

            return json;
        });

        services.AddTransient<MainViewModel>();
        services.AddTransient<GeneralSettingViewModel>();
        services.AddTransient<ThemeSettingViewModel>();
        services.AddTransient<AppUpdateSettingViewModel>();
        services.AddTransient<AboutUsSettingViewModel>();
        services.AddTransient<HomeLandingViewModel>();
        services.AddTransient<SettingsViewModel>();
        services.AddTransient<BreadCrumbBarViewModel>();

        services.AddTransient<ServerViewModel>();
        services.AddTransient<IMDBDetailViewModel>();
        services.AddTransient<SubsceneViewModel>();
        services.AddTransient<SubsceneDetailViewModel>();
        services.AddTransient<MediaViewModel>();
        services.AddTransient<MediaDetailsViewModel>();
        services.AddTransient<BoxOfficeViewModel>();
        services.AddTransient<BoxOfficeDetailViewModel>();

        services.AddTransient<BackupSettingViewModel>();
        services.AddTransient<LayoutSettingViewModel>();
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

        currentWindow.Title = currentWindow.AppWindow.Title = $"{AppName} v{AppVersion}";
        currentWindow.AppWindow.SetIcon("Assets/icon.ico");

        currentWindow.Activate();

        AppCenter.Start(Constants.AppCenterKey, typeof(Analytics), typeof(Crashes));

        if (Settings.UseDeveloperMode)
        {
            ConfigureLogger();
        }

        if (Settings.SubtitleLanguagesCollection == null || Settings.SubtitleLanguagesCollection.Count == 0)
        {
            Settings.SubtitleLanguagesCollection = new(SubtitleLanguageCollection().Where(x => x.IsSelected == true).Select(x => x.Content.ToString()));
        }

        UnhandledException += App_UnhandledException;
    }

    private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        Logger?.Error(e.Exception, "UnhandledException");
    }
}

