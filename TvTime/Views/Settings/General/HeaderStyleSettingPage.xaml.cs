using TvTime.ViewModels;

namespace TvTime.Views;
public sealed partial class HeaderStyleSettingPage : Page
{
    public HeaderStyleSettingViewModel ViewModel { get; }
    public string BreadCrumbBarItemText { get; set; }

    public HeaderStyleSettingPage()
    {
        ViewModel = App.Current.Services.GetService<HeaderStyleSettingViewModel>();
        this.InitializeComponent();
        Loaded += HeaderStyleSettingPage_Loaded;
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        BreadCrumbBarItemText = ((NavigationArgs) e.Parameter).Parameter as string;
    }

    private void HeaderStyleSettingPage_Loaded(object sender, RoutedEventArgs e)
    {
        var headerStyle = Settings.SettingsCardHeaderStyle;
        cmbHeader.SelectedItem = cmbHeader.Items.FirstOrDefault(x => (string) x == headerStyle);
    }

    private void FontSize_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        if (Settings.UseHeaderCustomFontSize && !double.IsNaN(sender.Value))
        {
            ViewModel.PreviewFontSize = sender.Value;
        }
    }
}
