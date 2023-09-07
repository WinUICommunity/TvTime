using CommunityToolkit.Labs.WinUI;

using TvTime.ViewModels;

namespace TvTime.Views;
public sealed partial class MediaPage : Page
{
    public PageOrDirectoryType PageType { get; set; }
    public static MediaPage Instance { get; set; }
    public MediaViewModel ViewModel { get; }
    public MediaPage()
    {
        this.InitializeComponent();
        ViewModel = App.GetService<MediaViewModel>();
        this.InitializeComponent();
        Instance = this;
        MediaItemsView.ItemTemplate = GetItemsViewDataTemplate(this);
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        var pageType = (e.Parameter as DataItem).Parameter?.ToString();
        this.PageType = ApplicationHelper.GetEnum<PageOrDirectoryType>(pageType);
    }

    public List<TokenItem> GetTokenSelectedItems()
    {
        return Token.SelectedItems.Cast<TokenItem>().ToList();
    }

    private void token_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        TokenViewSelectionChanged(ViewModel, Token, e, OnTokenFilter);
    }

    private void OnTokenFilter()
    {
        ViewModel.DataListACV.Filter += (item) =>
        {
            var query = (MediaItem) item;
            return Token.SelectedItems.Cast<TokenItem>().Any(x => query.Server.Contains(x.Content.ToString()));
        };
    }

    private void ItemUserControl_Loading(FrameworkElement sender, object args)
    {
        var item = sender as ItemUserControl;
        item.ViewModel = ViewModel;
        item.SettingsCardCommand = ViewModel.SettingsCardCommand;
        item.SettingsCardDoubleClickCommand = ViewModel.SettingsCardDoubleClickCommand;
        var conv = new MediaHeaderIconConverter();
        var headerIcon = conv.Convert(PageType, null, null, null);
        if (headerIcon != null)
        {
            item.HeaderIcon = (BitmapIcon) headerIcon;
        }
    }
}
