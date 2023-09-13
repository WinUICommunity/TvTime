using CommunityToolkit.Labs.WinUI;

namespace TvTime.Views;
public sealed partial class SubsceneDetailPage : Page
{
    public static SubsceneDetailPage Instance { get; set; }
    public SubsceneDetailViewModel ViewModel { get; }
    public SubsceneDetailPage()
    {
        ViewModel = App.GetService<SubsceneDetailViewModel>();
        this.InitializeComponent();
        Instance = this;
    }

    public List<TokenItem> GetTokenViewSelectedItems()
    {
        return LanguageTokenView.SelectedItems.Cast<TokenItem>().ToList();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        var args = (SubtitleModel) e.Parameter;
        ViewModel.rootMedia = args;
        ViewModel.BreadcrumbBarList?.Clear();
    }

    private void LanguageTokenView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        TokenViewSelectionChanged(ViewModel, LanguageTokenView, e, OnLanguageTokenFilter);
    }

    private void OnLanguageTokenFilter()
    {
        ViewModel.DataListACV.Filter += (item) =>
        {
            var query = (SubtitleModel) item;
            return LanguageTokenView.SelectedItems.Cast<TokenItem>().Any(x => query.Language.Contains(x.Content.ToString()));
        };
    }

    private void ItemUserControl_Loading(FrameworkElement sender, object args)
    {
        var item = sender as ItemUserControl;
        item.ViewModel = ViewModel;
        item.SettingsCardCommand = ViewModel.SettingsCardCommand;
        item.SettingsCardDoubleClickCommand = ViewModel.SettingsCardDoubleClickCommand;
    }
}
