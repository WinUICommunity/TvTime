using TvTime.ViewModels;

namespace TvTime.Views;
public sealed partial class GeneralSettingPage : Page
{
    public GeneralSettingViewModel ViewModel { get; }
    public string BreadCrumbBarItemText { get; set; }
    public GeneralSettingPage()
    {
        ViewModel = App.Current.Services.GetService<GeneralSettingViewModel>();
        this.InitializeComponent();
        Loaded += GeneralSettingPage_Loaded;
    }

    private void GeneralSettingPage_Loaded(object sender, RoutedEventArgs e)
    {
        var iconPack = Settings.IconPack;
        cmbIconPack.SelectedItem = cmbIconPack.Items.FirstOrDefault(x => ((ComboBoxItem) x).Tag.ToString() == iconPack.ToString());
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        BreadCrumbBarItemText = ((NavigationArgs) e.Parameter).Parameter as string;
    }
}
