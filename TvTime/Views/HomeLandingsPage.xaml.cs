namespace TvTime.Views;
public sealed partial class HomeLandingsPage : Page
{
    public string TvTimeVersion { get; set; }

    public HomeLandingsPage()
    {
        this.InitializeComponent();
        TvTimeVersion = $"TvTime v{VersionHelper.GetVersion()}";
    }

    private void mainLandingsPage_Loaded(object sender, RoutedEventArgs e)
    {
        mainLandingsPage.GetDataAsync("DataModel/ControlInfoData.json", IncludedInBuildMode.CheckBasedOnIncludedInBuildProperty);
    }

    private void mainLandingsPage_OnItemClick(object sender, RoutedEventArgs e)
    {
        var args = (ItemClickEventArgs) e;
        var item = (ControlInfoDataItem) args.ClickedItem;
        var assembly = Assembly.Load(item.ApiNamespace);
        NavigationViewHelper.GetCurrent().Navigate(assembly.GetType(item.UniqueId));
    }

    private void settingsTile_OnItemClick(object sender, RoutedEventArgs e)
    {
        NavigationViewHelper.GetCurrent().Navigate(typeof(SettingsPage));
    }

    private void aboutTile_OnItemClick(object sender, RoutedEventArgs e)
    {
        NavigationViewHelper.GetCurrent().Navigate(typeof(AboutPage));
    }
}
