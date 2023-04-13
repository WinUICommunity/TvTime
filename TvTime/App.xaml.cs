namespace TvTime;

public partial class App : Application
{
    public ThemeManager themeManager { get; private set; }
    public App()
    {
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

        this.InitializeComponent();
        CreateDirectory();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        m_window = new MainWindow();
        themeManager = ThemeManager.Initialize(m_window, new ThemeOptions
        {
            TitleBarCustomization = new TitleBarCustomization
            {
                TitleBarType = TitleBarType.AppWindow
            }
        });
        m_window.Activate();
    }

    private Window m_window;
}
