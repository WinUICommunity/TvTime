namespace TvTime.Views;
public sealed partial class GeneralSettingPage : Page
{
    public GeneralSettingViewModel ViewModel { get; }
    public string BreadCrumbBarItemText { get; set; }
    public GeneralSettingPage()
    {
        ViewModel = App.GetService<GeneralSettingViewModel>();
        this.InitializeComponent();
    }

    private void TGRegex_Toggled(object sender, RoutedEventArgs e)
    {
        if (TGRegex.IsOn)
        {
            Settings.FileNameRegex = Constants.FileNameRegex;
        }
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        BreadCrumbBarItemText = e.Parameter as string;
    }
}
