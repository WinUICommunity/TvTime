using CommunityToolkit.Labs.WinUI;

using TvTime.ViewModels;

namespace TvTime.Views;

public sealed partial class MediaUserControl : UserControl
{
    public PageOrDirectoryType PageType
    {
        get => (PageOrDirectoryType) GetValue(PageTypeProperty);
        set => SetValue(PageTypeProperty, value);
    }

    public static readonly DependencyProperty PageTypeProperty =
        DependencyProperty.Register("PageType", typeof(PageOrDirectoryType), typeof(MediaUserControl), new PropertyMetadata(null));

    public static MediaUserControl Instance { get; set; }
    public MediaViewModel ViewModel { get; }

    public MediaUserControl()
    {
        ViewModel = App.Current.Services.GetService<MediaViewModel>();
        this.InitializeComponent();
        Instance = this;
        DataContext = this;
    }

    public List<TokenItem> GetTokenSelectedItems()
    {
        return Token.SelectedItems.Cast<TokenItem>().ToList();
    }

    private void token_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ViewModel.DataListACV == null)
        {
            return;
        }

        if (Token != null)
        {
            dynamic selectedItem = e.AddedItems.Count > 0 ? e.AddedItems[0] as TokenItem : null;

            if (selectedItem == null)
            {
                selectedItem = e.RemovedItems.Count > 0 ? e.RemovedItems[0] as TokenItem : null;
            }

            if (selectedItem == null)
            {
                return;
            }

            if (Token.SelectedItems.Count == 0)
            {
                var allItem = Token.Items[0] as TokenItem;
                allItem.IsSelected = true;
                selectedItem = allItem;
            }

            if (selectedItem.Content.ToString().Equals(Constants.ALL_FILTER) && selectedItem.IsSelected)
            {
                foreach (TokenItem item in Token.Items)
                {
                    if (item.Content.ToString().Equals(Constants.ALL_FILTER))
                    {
                        continue;
                    }
                    item.IsSelected = false;
                }

                ViewModel.DataListACV.Filter = null;
            }
            else if (!selectedItem.Content.ToString().Equals(Constants.ALL_FILTER))
            {
                foreach (TokenItem item in Token.Items)
                {
                    if (item.Content.ToString().Equals(Constants.ALL_FILTER) && item.IsSelected)
                    {
                        item.IsSelected = false;
                    }
                    break;
                }

                ViewModel.DataListACV.Filter = OnTokenFilter;
            }

            ViewModel.DataListACV.RefreshFilter();
        }
    }

    private bool OnTokenFilter(object item)
    {
        var query = (MediaItem) item;
        return Token.SelectedItems.Cast<TokenItem>().Any(x => query.Server.Contains(x.Content.ToString()));
    }
}
