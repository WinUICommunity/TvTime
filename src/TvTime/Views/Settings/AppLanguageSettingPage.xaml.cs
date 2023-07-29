using TvTime.ViewModels;

namespace TvTime.Views;
public sealed partial class AppLanguageSettingPage : Page
{
    public AppLanguageSettingViewModel ViewModel { get; }
    public string BreadCrumbBarItemText { get; set; }
    public AppLanguageSettingPage()
    {
        ViewModel = App.GetService<AppLanguageSettingViewModel>();
        this.InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        BreadCrumbBarItemText = e.Parameter as string;
    }
}
