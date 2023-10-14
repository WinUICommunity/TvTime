namespace TvTime.Views;

public sealed partial class HomeLandingPage : Page
{
    public string AppInfo { get; set; }
    public HomeLandingViewModel ViewModel { get; }
    public static HomeLandingPage Instance { get; set; }
    public HomeLandingPage()
    {
        ViewModel = App.GetService<HomeLandingViewModel>();
        this.InitializeComponent();
        Instance = this;
        AppInfo = $"{App.Current.AppName} v{App.Current.AppVersion}";
        UpdateHomeLandingPageCornerRadius();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        allLandingPage.GetData(ViewModel.JsonNavigationViewService.DataSource);
        allLandingPage.OrderBy(i => i.Title);
    }

    public void UpdateHomeLandingPageCornerRadius()
    {
        switch (Settings.NavigationViewPaneDisplayMode)
        {
            case NavigationViewPaneDisplayMode.Auto:
            case NavigationViewPaneDisplayMode.Left:
                allLandingPage.HeaderGridCornerRadius = new CornerRadius(8, 0, 0, 0);
                break;
            case NavigationViewPaneDisplayMode.Top:
            case NavigationViewPaneDisplayMode.LeftCompact:
                allLandingPage.HeaderGridCornerRadius = new CornerRadius(0);
                break;

            case NavigationViewPaneDisplayMode.LeftMinimal:
                break;
        }
    }
}

