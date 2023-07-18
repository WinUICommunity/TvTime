namespace TvTime.Views;
public sealed partial class MainPage : Page
{
    public string TvTimeVersion { get; set; } = $"v{App.Current.TvTimeVersion} - Preview";
    public static MainPage Instance { get; set; }
    private AutoSuggestBoxTextChangedEventArgs args;
    public MainPage()
    {
        this.InitializeComponent();
        appTitleBar.Window = App.currentWindow;
        Instance = this;
        Loaded += MainPage_Loaded;

        App.Current.NavigationManager = NavigationManager.Initialize(NavView, new NavigationViewOptions
        {
            DefaultPage = typeof(HomeLandingsPage),
            SettingsPage = typeof(SettingsPage),
            JsonOptions = new JsonOptions
            {
                JsonFilePath = "DataModel/ControlInfoData.json"
            }
        }, NavFrame, ControlsSearchBox, new AutoSuggestBoxOptions
        {
            NoResultImage = "ms-appx:///Assets/Fluent/icon.png"
        });
    }

    private void MainPage_Loaded(object sender, RoutedEventArgs e)
    {
        var settings = (NavigationViewItem) NavView.SettingsItem;
        if (settings != null)
        {
            settings.Icon = new BitmapIcon { UriSource = new Uri("ms-appx:///Assets/Fluent/settings.png"), ShowAsMonochrome = false };
        }
        ElementSoundPlayer.State = Settings.UseSound ? ElementSoundPlayerState.On : ElementSoundPlayerState.Off;
    }

    private void appTitleBar_BackButtonClick(object sender, RoutedEventArgs e)
    {
        if (NavFrame.CanGoBack)
        {
            NavFrame.GoBack();
        }
    }

    private void appTitleBar_PaneButtonClick(object sender, RoutedEventArgs e)
    {
        NavView.IsPaneOpen = !NavView.IsPaneOpen;
    }

    private void NavFrame_Navigated(object sender, NavigationEventArgs e)
    {
        appTitleBar.IsBackButtonVisible = NavFrame.CanGoBack;
    }

    private void TxtSearch_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        // Subtitles can not be searched realtime because of server issues
        var rootFrame = App.Current.NavigationManager.Frame;
        dynamic viewModel = null;
        if (rootFrame.Content is SubscenePage)
        {
            viewModel = SubscenePage.Instance.ViewModel;
            viewModel.setQuery(TxtSearch.Text);
            viewModel.OnQuerySubmitted();
        }
        else
        {
            if (this.args != null)
            {
                Search();
            }
        }
    }

    private void txtSearch_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        this.args = args;

        if (Settings.UseRealTimeSearch)
        {
            Search();
        }
        else
        {
            if (string.IsNullOrEmpty(TxtSearch.Text))
            {
                Search();
            }
        }
    }

    private void Search()
    {
        var viewModel = GetCurrentViewModel();
        if (viewModel != null)
        {
            viewModel.Search(TxtSearch, this.args);
        }
    }

    private dynamic GetCurrentViewModel()
    {
        var rootFrame = App.Current.NavigationManager.Frame;
        dynamic root = rootFrame.Content;
        dynamic viewModel = null;
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
            var frameContent = ServersPage.Instance.GetFrame().Content;
            if (frameContent is MediaServersPage)
            {
                viewModel = (MediaServersPage.Instance.Content as ServerUserControl).ViewModel;
            }
            else if (frameContent is SubtitleServersPage)
            {
                viewModel = (SubtitleServersPage.Instance.Content as ServerUserControl).ViewModel;
            }
        }
        else if (rootFrame.Content is IMDBDetailsPage)
        {
            viewModel = IMDBDetailsPage.Instance.ViewModel;
            viewModel.setQuery(TxtSearch.Text);
            viewModel.OnQuerySubmitted();
        }
        else if (rootFrame.Content is SubsceneDetailPage)
        {
            viewModel = SubsceneDetailPage.Instance.ViewModel;
        }

        return viewModel != null && rootFrame.Content is not IMDBDetailsPage ? viewModel : null;
    }

    public AutoSuggestBox GetTxtSearch()
    {
        return TxtSearch;
    }

    private void ThemeButton_Click(object sender, RoutedEventArgs e)
    {
        var element = App.currentWindow.Content as FrameworkElement;

        if (element.ActualTheme == ElementTheme.Light)
        {
            element.RequestedTheme = ElementTheme.Dark;
        }
        else if (element.ActualTheme == ElementTheme.Dark)
        {
            element.RequestedTheme = ElementTheme.Light;
        }
    }

    private void TxtSearch_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        TxtSearch.Text = args.SelectedItem.ToString();
        TxtSearch.IsSuggestionListOpen = true;
    }

    public void ClearTxtSearch()
    {
        TxtSearch.ItemsSource = null;
        TxtSearch.Text = string.Empty;
    }
}
