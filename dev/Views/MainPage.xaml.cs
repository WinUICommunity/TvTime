namespace TvTime.Views;

public sealed partial class MainPage : Page
{
    public MainViewModel ViewModel { get; }
    public static MainPage Instance { get; set; }
    public MainPage()
    {
        ViewModel = App.GetService<MainViewModel>();
        this.InitializeComponent();
        Instance = this;
        appTitleBar.Window = App.currentWindow;
        ViewModel.JsonNavigationViewService.Initialize(NavView, NavFrame);
        ViewModel.JsonNavigationViewService.ConfigJson("Assets/NavViewMenu/AppData.json");

        NavFrame.Navigating += (s, e) =>
        {
            var page = NavFrame.Content as Page;
            if (page != null && e.SourcePageType != typeof(MediaDetailPage))
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

    private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        var viewModel = NavFrame.GetPageViewModel();
        if (viewModel != null && viewModel is ITitleBarAutoSuggestBoxAware titleBarAutoSuggestBoxAware)
        {
            titleBarAutoSuggestBoxAware.OnAutoSuggestBoxTextChanged(sender, args);
        }
    }

    private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        var viewModel = NavFrame.GetPageViewModel();
        if (viewModel != null && viewModel is ITitleBarAutoSuggestBoxAware titleBarAutoSuggestBoxAware)
        {
            titleBarAutoSuggestBoxAware.OnAutoSuggestBoxQuerySubmitted(sender, args);
        }
    }

    public void RefreshNavigationViewPaneDisplayMode()
    {
        NavView.PaneDisplayMode = Settings.NavigationViewPaneDisplayMode;
    }

    private void NavView_DisplayModeChanged(NavigationView sender, NavigationViewDisplayModeChangedEventArgs args)
    {
        if (NavView.PaneDisplayMode == NavigationViewPaneDisplayMode.Top)
        {
            appTitleBar.IsPaneButtonVisible = false;
        }
        else
        {
            appTitleBar.IsPaneButtonVisible = true;
        }
    }

    private void NavViewPaneDisplayModeButton_Click(object sender, RoutedEventArgs e)
    {
        switch (NavView.PaneDisplayMode)
        {
            case NavigationViewPaneDisplayMode.Auto:
                NavView.PaneDisplayMode = NavigationViewPaneDisplayMode.Left;
                break;
            case NavigationViewPaneDisplayMode.Left:
                NavView.PaneDisplayMode = NavigationViewPaneDisplayMode.Top;
                break;
            case NavigationViewPaneDisplayMode.Top:
                NavView.PaneDisplayMode = NavigationViewPaneDisplayMode.LeftCompact;
                break;
            case NavigationViewPaneDisplayMode.LeftCompact:
                NavView.PaneDisplayMode = NavigationViewPaneDisplayMode.LeftMinimal;
                break;
            case NavigationViewPaneDisplayMode.LeftMinimal:
                NavView.PaneDisplayMode = NavigationViewPaneDisplayMode.Auto;
                break;
        }

        Settings.NavigationViewPaneDisplayMode = NavView.PaneDisplayMode;

        if (HomeLandingPage.Instance != null)
        {
            HomeLandingPage.Instance.UpdateHomeLandingPageCornerRadius();
        }
    }
}

