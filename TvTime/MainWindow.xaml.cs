namespace TvTime;

public sealed partial class MainWindow : Window
{
    public static MainWindow Instance { get; private set; }
    public string TvTimeVersion { get; set; }
    public MainWindow()
    {
        this.InitializeComponent();
        Instance = this;
        TitleBarHelper.Initialize(this, TitleTextBlock, AppTitleBar, LeftPaddingColumn, IconColumn, TitleColumn, LeftDragColumn, SearchColumn, RightDragColumn, RightPaddingColumn);
        TvTimeVersion = $"TvTime v{VersionHelper.GetVersion()}";

        NavigationViewHelper.GetCurrent()
                                .WithAutoSuggestBox(controlsSearchBox)
                                .WithSettingsPage(typeof(SettingsPage))
                                .WithDefaultPage(typeof(HomeLandingsPage))
                                .Build("DataModel/ControlInfoData.json", rootFrame, NavigationViewControl);
    }
    private void controlsSearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        NavigationViewHelper.GetCurrent().AutoSuggestBoxQuerySubmitted(args);
    }

    private void OnNavigationViewSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        NavigationViewHelper.GetCurrent().OnNavigationViewSelectionChanged(args);
    }
    private void NavigationViewControl_Loaded(object sender, RoutedEventArgs e)
    {
        var settings = (NavigationViewItem) NavigationViewControl.SettingsItem;
        settings.Icon = new BitmapIcon { UriSource = new Uri("ms-appx:///Assets/Images/Fluent/settings.png"), ShowAsMonochrome = false };
    }
}
