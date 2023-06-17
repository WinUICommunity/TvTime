using TvTime.ViewModels;

namespace TvTime.Views;
public sealed partial class DescriptionStyleSettingPage : Page
{
    public DescriptionStyleSettingViewModel ViewModel { get; }
    public GeneralSettingViewModel GeneralViewModel { get; }

    public DescriptionStyleSettingPage()
    {
        ViewModel = App.Current.Services.GetService<DescriptionStyleSettingViewModel>();
        GeneralViewModel = GeneralSettingPage.Instance.ViewModel;
        GeneralViewModel.BreadCrumbBarCollection.Add("Description Style");
        this.InitializeComponent();
        DataContext = this;
        Loaded += DescriptionStyleSettingPage_Loaded;
    }

    private void DescriptionStyleSettingPage_Loaded(object sender, RoutedEventArgs e)
    {
        var descType = Settings.DescriptionType;
        cmbDescriptionType.SelectedItem = cmbDescriptionType.Items.FirstOrDefault(x => ((ComboBoxItem) x).Tag.ToString() == descType.ToString());

        var descriptionStyle = Settings.SettingsCardDescriptionStyle;
        cmbDescription.SelectedItem = cmbDescription.Items.FirstOrDefault(x => (string) x == descriptionStyle);
    }

    private void FontSize_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        if (Settings.UseDescriptionCustomFontSize && !double.IsNaN(sender.Value))
        {
            ViewModel.PreviewFontSize = sender.Value;
        }
    }
}
