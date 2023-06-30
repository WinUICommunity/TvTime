using CommunityToolkit.Labs.WinUI;

using TvTime.ViewModels;

namespace TvTime.Views;
public sealed partial class SubsceneDetailPage : Page
{
    public static SubsceneDetailPage Instance { get; set; }
    public SubsceneDetailViewModel ViewModel { get; }
    public SubsceneDetailPage()
    {
        ViewModel = App.Current.Services.GetService<SubsceneDetailViewModel>();
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
        var args = e.Parameter as NavigationArgs;
        var item = (SubsceneModel) args.Parameter;
        ViewModel.rootTvTimeItem = item;
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
