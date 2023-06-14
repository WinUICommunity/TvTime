using TvTime.ViewModels;

namespace TvTime.Views;
public sealed partial class AboutUsSettingPage : Page
{
    public AboutUsSettingViewModel ViewModel { get; }
    public string BreadCrumbBarItemText { get; set; }
    public AboutUsSettingPage()
    {
        ViewModel = App.Current.Services.GetService<AboutUsSettingViewModel>();
        this.InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        BreadCrumbBarItemText = ((NavigationArgs) e.Parameter).Parameter as string;
    }
}
