namespace TvTime.Views;
public sealed partial class HeaderStyleSettingPage : Page
{
    public HeaderStyleSettingViewModel ViewModel { get; }
    public LayoutSettingViewModel LayoutSettingViewModel { get; }

    public HeaderStyleSettingPage()
    {
        ViewModel = App.GetService<HeaderStyleSettingViewModel>();
        LayoutSettingViewModel = LayoutSettingPage.Instance.ViewModel;
        LayoutSettingViewModel.BreadCrumbBarCollection.Add("Header Style");
        this.InitializeComponent();
        Loaded += HeaderStyleSettingPage_Loaded;
    }

    private void HeaderStyleSettingPage_Loaded(object sender, RoutedEventArgs e)
    {
        var headerStyle = Settings.HeaderTextBlockStyle;
        CmbHeader.SelectedItem = CmbHeader.Items.FirstOrDefault(x => (string) x == headerStyle);
    }
}
