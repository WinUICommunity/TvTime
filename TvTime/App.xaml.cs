using Microsoft.UI.Xaml.Media;

using TvTime.ViewModels;

namespace TvTime;

public partial class App : Application
{
    public NavigationManager NavigationManager { get; set; }
    public ThemeManager ThemeManager { get; set; }
    public string TvTimeVersion { get; set; }
    public Window Window { get; set; }
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
        services.AddTransient<SubtitleViewModel>();
        services.AddTransient<SubsceneViewModel>();

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
        m_window = new MainWindow();
        this.Window = m_window;
        Frame rootFrame = new Frame();
        m_window.Content = rootFrame;
        rootFrame.Navigate(typeof(MainPage));
        ThemeManager = ThemeManager.Initialize(m_window, new ThemeOptions
        {
            BackdropFallBackColorForWindows10 = Current.Resources["ApplicationPageBackgroundThemeBrush"] as Brush,
            TitleBarCustomization = new TitleBarCustomization
            {
                TitleBarType = TitleBarType.AppWindow
            }
        });

        if (Settings.SubtitleLanguagesCollection == null || Settings.SubtitleLanguagesCollection.Count == 0)
        {
            Settings.SubtitleLanguagesCollection = SubtitleLanguageCollection();
        }

        if (Settings.SubtitleQualityCollection == null || Settings.SubtitleQualityCollection.Count == 0)
        {
            Settings.SubtitleQualityCollection = SubtitleQualityCollection();
        }

        m_window.Activate();
    }

    private Window m_window;
}
