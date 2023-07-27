using TvTime.ViewModels;

namespace TvTime.Views;
public sealed partial class HomeLandingsPage : Page
{
    public string TvTimeVersion { get; set; }
    public HomeLandingsViewModel ViewModel { get; }

    public HomeLandingsPage()
    {
        this.InitializeComponent();
        ViewModel = App.Current.Services.GetService<HomeLandingsViewModel>();
        TvTimeVersion = $"TvTime v{App.Current.TvTimeVersion}";
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        allLandingPage.GetData(App.Current.JsonNavigationViewService.DataSource);
    }
}
