namespace TvTime.Views;
public sealed partial class DescriptionStyleSettingPage : Page
{
    public DescriptionStyleSettingViewModel ViewModel { get; }
    public LayoutSettingViewModel LayoutSettingViewModel { get; }

    public DescriptionStyleSettingPage()
    {
        ViewModel = App.GetService<DescriptionStyleSettingViewModel>();
        LayoutSettingViewModel = LayoutSettingPage.Instance.ViewModel;
        LayoutSettingViewModel.BreadCrumbBarCollection.Add("Description Style");
        this.InitializeComponent();
        Loaded += DescriptionStyleSettingPage_Loaded;
    }

    private void DescriptionStyleSettingPage_Loaded(object sender, RoutedEventArgs e)
    {
        var descriptionStyle = Settings.DescriptionTextBlockStyle;
        CmbDescription.SelectedItem = CmbDescription.Items.FirstOrDefault(x => (string) x == descriptionStyle);
    }
}
