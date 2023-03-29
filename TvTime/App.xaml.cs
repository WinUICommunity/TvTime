namespace TvTime;

public partial class App : Application
{
    public static ThemeManager themeManager { get; private set; }
    public App()
    {
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

        this.InitializeComponent();
        CreateDirectory();
        LaunchTask(m_window);
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        m_window = new MainWindow();
        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(m_window);
        //m_sc.CenterToScreen(hWnd);

        themeManager = ThemeManager.Initialize(m_window, BackdropType.MicaAlt);
        m_window.Activate();
        //m_sc.HideSplash();
    }
    //public SplashScreen m_sc;

    private async void LaunchTask(Window window)
    {
        //m_sc = new SplashScreen();
        //m_sc.Initialize();
        //IntPtr hBitmap = await m_sc.GetBitmap(@"Assets\Images\aboutCover.jpg");
        //m_sc.DisplaySplash(IntPtr.Zero, hBitmap);
    }

    private Window m_window;
}
