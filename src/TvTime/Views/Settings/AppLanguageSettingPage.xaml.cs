using TvTime.ViewModels;

namespace TvTime.Views;
public sealed partial class AppLanguageSettingPage : Page
{
    public AppLanguageSettingViewModel ViewModel { get; }
    public string BreadCrumbBarItemText { get; set; }
    public AppLanguageSettingPage()
    {
        ViewModel = App.GetService<AppLanguageSettingViewModel>();
        this.InitializeComponent();

        Loaded += AppLanguageSettingPage_Loaded;
    }

    private void AppLanguageSettingPage_Loaded(object sender, RoutedEventArgs e)
    {
        var language = Settings.TvTimeLanguage;
        CmbLanguage.SelectedItem = CmbLanguage.Items.FirstOrDefault(x => ((TvTimeLanguage) x).Title.Equals(language.Title) && ((TvTimeLanguage) x).LanguageCode.Equals(language.LanguageCode));
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        BreadCrumbBarItemText = e.Parameter as string;
    }
}
