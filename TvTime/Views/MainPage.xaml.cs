namespace TvTime.Views;
public sealed partial class MainPage : Page
{
    public string TvTimeVersion { get; set; }
    public static MainPage Instance { get; set; }

    public MainPage()
    {
        this.InitializeComponent();
        Instance = this;
        Loaded += MainPage_Loaded;

        var titleBarHelper = TitleBarHelper.Initialize(App.Current.Window, TitleTextBlock, AppTitleBar, LeftPaddingColumn, IconColumn, TitleColumn, LeftDragColumn, SearchColumn, RightDragColumn, RightPaddingColumn);
        titleBarHelper.LeftPadding = 48;
        titleBarHelper.RightPadding = -100;

        TvTimeVersion = $"TvTime v{(Application.Current as App).TvTimeVersion}";

        App.Current.NavigationManager = NavigationManager.Initialize(NavigationViewControl, new NavigationViewOptions
        {
            DefaultPage = typeof(HomeLandingsPage),
            SettingsPage = typeof(SettingsPage),
            JsonOptions = new JsonOptions
            {
                JsonFilePath = "DataModel/ControlInfoData.json"
            }
        }, RootFrame, ControlsSearchBox, new AutoSuggestBoxOptions
        {
            NoResultImage = "ms-appx:///Assets/Fluent/icon.png"
        });
    }

    private void MainPage_Loaded(object sender, RoutedEventArgs e)
    {
        var settings = (NavigationViewItem) NavigationViewControl.SettingsItem;
        settings.Icon = new BitmapIcon { UriSource = new Uri("ms-appx:///Assets/Fluent/settings.png"), ShowAsMonochrome = false };
    }

    private void txtSearch_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        var rootFrame = App.Current.NavigationManager.Frame;
        dynamic root = rootFrame.Content;
        dynamic viewModel = null;
        TxtSearch.ItemsSource = null;
        if (root is AnimesPage || root is MoviesPage || root is SeriesPage)
        {
            viewModel = MediaUserControl.Instance.ViewModel;
        }
        else if (rootFrame.Content is DetailPage)
        {
            viewModel = DetailPage.Instance.ViewModel;
        }
        else if (rootFrame.Content is ServersPage)
        {
            viewModel = ServersPage.Instance.ViewModel;
        }

        if (viewModel != null)
        {
            viewModel.Search(sender, args);
        }
    }

    public AutoSuggestBox GetTxtSearch()
    {
        return TxtSearch;
    }
}
