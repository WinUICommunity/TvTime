namespace TvTime.ViewModels;
public partial class MainViewModel : ObservableObject
{
    public IJsonNavigationViewService JsonNavigationViewService;
    public MainViewModel(IJsonNavigationViewService jsonNavigationViewService)
    {
        JsonNavigationViewService = jsonNavigationViewService;
        jsonNavigationViewService.ConfigJson("DataModel/AppData.json");
        jsonNavigationViewService.ConfigDefaultPage(typeof(HomeLandingsPage));
        jsonNavigationViewService.ConfigSettingsPage(typeof(SettingsPage));
    }
}
