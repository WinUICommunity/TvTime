using TvTime.ViewModels;

namespace TvTime.Views;
public sealed partial class HeaderStyleSettingPage : Page
{
    public HeaderStyleSettingViewModel ViewModel { get; }
    public LayoutSettingViewModel LayoutSettingViewModel { get; }

    public HeaderStyleSettingPage()
    {
        ViewModel = App.GetService<HeaderStyleSettingViewModel>();
        LayoutSettingViewModel = LayoutSettingPage.Instance.ViewModel;
        LayoutSettingViewModel.BreadCrumbBarCollection.Add("Header Style");
        this.InitializeComponent();
        Loaded += HeaderStyleSettingPage_Loaded;
    }

    private void HeaderStyleSettingPage_Loaded(object sender, RoutedEventArgs e)
    {
        var headerStyle = Settings.HeaderTextBlockStyle;
        CmbHeader.SelectedItem = CmbHeader.Items.FirstOrDefault(x => (string) x == headerStyle);
    }

    private void FontSize_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        if (Settings.UseCustomFontSizeForHeader && !double.IsNaN(sender.Value))
        {
            ViewModel.PreviewFontSize = sender.Value;
        }
    }
}
