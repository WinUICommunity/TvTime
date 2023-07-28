using CommunityToolkit.Labs.WinUI;

using TvTime.ViewModels;

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
        DataContext = this;
    }

    public List<TokenItem> GetLanguageTokenViewSelectedItems()
    {
        return LanguageTokenView.SelectedItems.Cast<TokenItem>().ToList();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        var args = (SubsceneModel) e.Parameter;
        ViewModel.rootTvTimeItem = args;
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
            var query = (SubsceneModel) item;
            return LanguageTokenView.SelectedItems.Cast<TokenItem>().Any(x => query.Language.Contains(x.Content.ToString()));
        };
    }
}
