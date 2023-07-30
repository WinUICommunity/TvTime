using TvTime.ViewModels;

namespace TvTime.Views;
public sealed partial class HomeLandingsPage : Page
{
    public string TvTimeVersion { get; set; }
    public HomeLandingsViewModel ViewModel { get; }

    public HomeLandingsPage()
    {
        ViewModel = App.GetService<HomeLandingsViewModel>();
        this.InitializeComponent();
        TvTimeVersion = $"TvTime v{App.Current.TvTimeVersion}";
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        allLandingPage.GetLocalizedData(ViewModel.JsonNavigationViewService.DataSource, App.Current.Localizer);
        allLandingPage.OrderBy(i => i.Title);
    }
}
