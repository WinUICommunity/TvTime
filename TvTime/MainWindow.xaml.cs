namespace TvTime;

public sealed partial class MainWindow : Window
{
    public static MainWindow Instance { get; private set; }
    public string TvTimeVersion { get; set; }
    public NavigationManager navigationManager { get; set; }
    public MainWindow()
    {
        this.InitializeComponent();
        Instance = this;
        TitleBarHelper.Initialize(this, TitleTextBlock, AppTitleBar, LeftPaddingColumn, IconColumn, TitleColumn, LeftDragColumn, SearchColumn, RightDragColumn, RightPaddingColumn);
        TvTimeVersion = $"TvTime v{VersionHelper.GetVersion()}";

        navigationManager = NavigationManager.Initialize(NavigationViewControl, new NavigationViewOptions
        {
            DefaultPage = typeof(HomeLandingsPage),
            SettingsPage = typeof(SettingsPage),
            JsonOptions = new JsonOptions
            {
                JsonFilePath = "DataModel/ControlInfoData.json"
            }
        }, rootFrame, controlsSearchBox);
    }
    private void NavigationViewControl_Loaded(object sender, RoutedEventArgs e)
    {
        var settings = (NavigationViewItem) NavigationViewControl.SettingsItem;
        settings.Icon = new BitmapIcon { UriSource = new Uri("ms-appx:///Assets/Images/Fluent/settings.png"), ShowAsMonochrome = false };
    }
}
