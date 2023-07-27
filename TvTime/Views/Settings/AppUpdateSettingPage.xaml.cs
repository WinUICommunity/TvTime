using TvTime.ViewModels;

namespace TvTime.Views;
public sealed partial class AppUpdateSettingPage : Page
{
    public AppUpdateSettingViewModel ViewModel { get; }
    public string BreadCrumbBarItemText { get; set; }

    public AppUpdateSettingPage()
    {
        ViewModel = App.Current.Services.GetService<AppUpdateSettingViewModel>();
        this.InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        BreadCrumbBarItemText = e.Parameter as string;
    }
}
