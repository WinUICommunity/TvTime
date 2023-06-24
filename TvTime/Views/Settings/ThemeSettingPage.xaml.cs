using TvTime.ViewModels;

namespace TvTime.Views;
public sealed partial class ThemeSettingPage : Page
{
    public ThemeSettingViewModel ViewModel { get; }
    public string BreadCrumbBarItemText { get; set; }

    public ThemeSettingPage()
    {
        ViewModel = App.Current.Services.GetService<ThemeSettingViewModel>();
        this.InitializeComponent();
        Loaded += ThemeSettingPage_Loaded;
    }

    private void ThemeSettingPage_Loaded(object sender, RoutedEventArgs e)
    {
        App.Current.ThemeManager.SetThemeComboBoxDefaultItem(CmbTheme);
        App.Current.ThemeManager.SetBackdropComboBoxDefaultItem(CmbBackdrop);
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        BreadCrumbBarItemText = ((NavigationArgs) e.Parameter).Parameter as string;
    }
}
