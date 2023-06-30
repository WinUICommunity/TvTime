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
}
