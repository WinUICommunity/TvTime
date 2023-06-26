namespace TvTime.Views;
public sealed partial class MediaItemUserControl : UserControl
{
    public PageOrDirectoryType PageType
    {
        get { return (PageOrDirectoryType) GetValue(PageTypeProperty); }
        set { SetValue(PageTypeProperty, value); }
    }

    public MediaItem MediaItem
    {
        get { return (MediaItem) GetValue(MediaItemProperty); }
        set { SetValue(MediaItemProperty, value); }
    }

    public object Description
    {
        get { return (object) GetValue(DescriptionProperty); }
        set { SetValue(DescriptionProperty, value); }
    }

    public static readonly DependencyProperty PageTypeProperty =
        DependencyProperty.Register("PageType", typeof(PageOrDirectoryType), typeof(MediaItemUserControl), new PropertyMetadata(default(PageOrDirectoryType)));

    public static readonly DependencyProperty MediaItemProperty =
        DependencyProperty.Register("MediaItem", typeof(MediaItem), typeof(MediaItemUserControl), new PropertyMetadata(default(MediaItem)));

    public static readonly DependencyProperty DescriptionProperty =
        DependencyProperty.Register("Description", typeof(object), typeof(MediaItemUserControl), new PropertyMetadata(default(object)));

    public MediaItemUserControl()
    {
        this.InitializeComponent();
    }

    private void SettingsCard_Click(object sender, RoutedEventArgs e)
    {
        if (!Settings.UseDoubleClickForNavigate)
        {
            NavigateToDetails(sender);
        }
    }

    private void NavigateToDetails(object sender)
    {
        var item = (sender as SettingsCard);
        var headerTextBlock = item?.Header as TextBlock;
        var title = headerTextBlock.Text?.Trim();
        var server = string.Empty;

        switch (Settings.DescriptionTemplate)
        {
            case DescriptionTemplateType.TextBlock:
                var descriptionTextBlock = item?.Description as TextBlock;
                server = descriptionTextBlock?.Text;
                break;
            case DescriptionTemplateType.HyperLink:
                var hyperLink = item?.Description as HyperlinkButton;
                var hyperLinkContent = hyperLink?.Content as TextBlock;
                server = hyperLinkContent?.Text;
                break;
        }

        var media = new MediaItem
        {
            Server = server,
            Title = title,
            ServerType = ApplicationHelper.GetEnum<ServerType>(PageType.ToString())
        };

        App.Current.NavigationManager.NavigateForJson(typeof(DetailPage), media);
    }

    private async void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
    {
        var item = (sender as MenuFlyoutItem);
        var mediaItem = (MediaItem) item?.DataContext;
        switch (item?.Tag?.ToString())
        {
            case "OpenWeb":
                var server = mediaItem.Server?.ToString();
                await Launcher.LaunchUriAsync(new Uri(server));
                break;
            case "IMDB":
                CreateIMDBDetailsWindow(mediaItem.Title);
                break;
        }
    }

    private void SettingsCard_DoubleTapped(object sender, Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
    {
        NavigateToDetails(sender);
    }
}
