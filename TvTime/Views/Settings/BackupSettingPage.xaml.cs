using TvTime.ViewModels;

namespace TvTime.Views;
public sealed partial class BackupSettingPage : Page
{
    public BackupSettingViewModel ViewModel { get; }
    public string BreadCrumbBarItemText { get; set; }
    public BackupSettingPage()
    {
        ViewModel = App.Current.Services.GetService<BackupSettingViewModel>();
        this.InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        BreadCrumbBarItemText = ((NavigationArgs) e.Parameter).Parameter as string;
    }
}
