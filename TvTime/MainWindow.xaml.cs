using WinUICommunity;

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
        var titleBarHelper = TitleBarHelper.Initialize(this, TitleTextBlock, AppTitleBar, LeftPaddingColumn, IconColumn, TitleColumn, LeftDragColumn, SearchColumn, RightDragColumn, RightPaddingColumn);
        titleBarHelper.LeftPadding = 48;
        titleBarHelper.RightPadding = -100;

        TvTimeVersion = $"TvTime v{VersionHelper.GetVersion()}";

        navigationManager = NavigationManager.Initialize(NavigationViewControl, new NavigationViewOptions
        {
            DefaultPage = typeof(HomeLandingsPage),
            SettingsPage = typeof(SettingsPage),
            JsonOptions = new JsonOptions
            {
                JsonFilePath = "DataModel/ControlInfoData.json"
            }
        }, rootFrame, controlsSearchBox, new AutoSuggestBoxOptions
        {
            NoResultImage = "ms-appx:///Assets/Images/icon.png"
        });
    }
    private void NavigationViewControl_Loaded(object sender, RoutedEventArgs e)
    {
        var settings = (NavigationViewItem) NavigationViewControl.SettingsItem;
        settings.Icon = new BitmapIcon { UriSource = new Uri("ms-appx:///Assets/Images/Fluent/settings.png"), ShowAsMonochrome = false };
    }

    private void txtSearch_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        dynamic root = rootFrame.Content;
        dynamic currentPage = null;
        if (root is AnimesPage || root is MoviesPage || root is SeriesPage)
        {
            currentPage = root.Content as LocalUserControl;
        }
        else if (rootFrame.Content is DetailPage)
        {
            currentPage = root as DetailPage;
        }
        else if (rootFrame.Content is ServersPage)
        {
            currentPage = root as ServersPage;
        }
        if (currentPage != null)
        {
            currentPage.Search(sender, args);
        }
    }

    public AutoSuggestBox GetTxtSearch()
    {
        return txtSearch;
    }
}
