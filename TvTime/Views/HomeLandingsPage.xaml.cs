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
        mainLandingsPage.GetDataAsync("DataModel/ControlInfoData.json", PathType.Relative);
    }

    private void mainLandingsPage_OnItemClick(object sender, RoutedEventArgs e)
    {
        var args = (ItemClickEventArgs) e;
        var item = (ControlInfoDataItem) args.ClickedItem;
        var assembly = Application.Current.GetType().Assembly;
        MainWindow.Instance.navigationManager.NavigateForJson(assembly.GetType(item.UniqueId));
    }

    private void settingsTile_OnItemClick(object sender, RoutedEventArgs e)
    {
        MainWindow.Instance.navigationManager.NavigateForJson(typeof(SettingsPage));
    }

    private void aboutTile_OnItemClick(object sender, RoutedEventArgs e)
    {
        MainWindow.Instance.navigationManager.NavigateForJson(typeof(AboutPage));
    }
}
