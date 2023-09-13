namespace TvTime.Views;
public sealed partial class LayoutSettingPage : Page
{
    public static LayoutSettingPage Instance { get; set; }
    public LayoutSettingViewModel ViewModel { get; }
    public LayoutSettingPage()
    {
        ViewModel = App.GetService<LayoutSettingViewModel>();
        this.InitializeComponent();
        Instance = this;
    }
}
