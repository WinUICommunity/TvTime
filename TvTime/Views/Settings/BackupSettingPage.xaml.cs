using TvTime.ViewModels;

namespace TvTime.Views;
public sealed partial class BackupSettingPage : Page
{
    public BackupSettingViewModel ViewModel { get; }
    public string BreadCrumbBarItemText { get; set; }
    public BackupSettingPage()
    {
        ViewModel = App.GetService<BackupSettingViewModel>();
        this.InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        BreadCrumbBarItemText = e.Parameter as string;
    }
}
