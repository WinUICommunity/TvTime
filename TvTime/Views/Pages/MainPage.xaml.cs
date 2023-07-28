using TvTime.ViewModels;

namespace TvTime.Views;
public sealed partial class MainPage : Page
{
    public string TvTimeVersion { get; set; } =
#if DEBUG
        $"v{App.Current.TvTimeVersion} - Preview";
#else
        $"v{App.Current.TvTimeVersion}";
#endif
    public static MainPage Instance { get; set; }
    private AutoSuggestBoxTextChangedEventArgs args;

    public MainViewModel ViewModel { get; }
    public MainPage()
    {
        ViewModel = App.GetService<MainViewModel>();
        this.InitializeComponent();
        appTitleBar.Window = App.currentWindow;
        Instance = this;

        ViewModel.JsonNavigationViewService.Initialize(NavView, NavFrame);
        ViewModel.JsonNavigationViewService.ConfigAutoSuggestBox(ControlsSearchBox, true, null, "ms-appx:///Assets/Fluent/icon.png");

        Loaded += MainPage_Loaded;

        NavFrame.Navigating += (s, e) =>
        {
            var page = NavFrame.Content as Page;
            if (page != null && e.SourcePageType != typeof(DetailPage))
            {
                page.NavigationCacheMode = NavigationCacheMode.Disabled;
                if (MediaPage.Instance != null && CanDisableCache(e.Parameter, MediaPage.Instance.PageType.ToString()))
                {
                    MediaPage.Instance.NavigationCacheMode = NavigationCacheMode.Disabled;
                }
            }
        };
    }

    private bool CanDisableCache(object parameter, string pageType)
    {
        var parameterItem = parameter as DataItem;

        if (parameterItem != null)
        {
            var item = parameterItem.Parameter?.ToString();
            if (!string.IsNullOrEmpty(item) && item.Equals(pageType))
            {
                return false;
            }
        }
        return true;
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
        var rootFrame = ViewModel.JsonNavigationViewService.Frame;
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
        var rootFrame = ViewModel.JsonNavigationViewService.Frame;
        dynamic root = rootFrame.Content;
        dynamic viewModel = null;
        if (root is MediaPage)
        {
            viewModel = MediaPage.Instance.ViewModel;
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
