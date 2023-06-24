using TvTime.ViewModels;

namespace TvTime.Views;
public sealed partial class HeaderStyleSettingPage : Page
{
    public HeaderStyleSettingViewModel ViewModel { get; }
    public GeneralSettingViewModel GeneralViewModel { get; }

    public HeaderStyleSettingPage()
    {
        ViewModel = App.Current.Services.GetService<HeaderStyleSettingViewModel>();
        GeneralViewModel = GeneralSettingPage.Instance.ViewModel;
        GeneralViewModel.BreadCrumbBarCollection.Add("Header Style");
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
