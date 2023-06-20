namespace TvTime.Views;
public sealed partial class LocalSettingsCardUserControl : UserControl
{
    public PageOrDirectoryType PageType
    {
        get { return (PageOrDirectoryType) GetValue(PageTypeProperty); }
        set { SetValue(PageTypeProperty, value); }
    }

    public LocalItem LocalItem
    {
        get { return (LocalItem) GetValue(LocalItemProperty); }
        set { SetValue(LocalItemProperty, value); }
    }

    public object Description
    {
        get { return (object) GetValue(DescriptionProperty); }
        set { SetValue(DescriptionProperty, value); }
    }

    public static readonly DependencyProperty PageTypeProperty =
        DependencyProperty.Register("PageType", typeof(PageOrDirectoryType), typeof(LocalSettingsCardUserControl), new PropertyMetadata(default(PageOrDirectoryType)));

    public static readonly DependencyProperty LocalItemProperty =
        DependencyProperty.Register("LocalItem", typeof(LocalItem), typeof(LocalSettingsCardUserControl), new PropertyMetadata(default(LocalItem)));

    public static readonly DependencyProperty DescriptionProperty =
        DependencyProperty.Register("Description", typeof(object), typeof(LocalSettingsCardUserControl), new PropertyMetadata(default(object)));

    public LocalSettingsCardUserControl()
    {
        this.InitializeComponent();
    }

    private void SettingsCard_Click(object sender, RoutedEventArgs e)
    {
        var setting = (sender as SettingsCard);
        var headerTextBlock = setting?.Header as TextBlock;
        var title = headerTextBlock.Text?.Trim();
        var server = string.Empty;

        switch (Settings.DescriptionTemplateType)
        {
            case DescriptionTemplateType.TextBlock:
                var descriptionTextBlock = setting?.Description as TextBlock;
                server = descriptionTextBlock?.Text;
                break;
            case DescriptionTemplateType.HyperLink:
                var hyperLink = setting?.Description as HyperlinkButton;
                var hyperLinkContent = hyperLink?.Content as TextBlock;
                server = hyperLinkContent?.Text;
                break;
        }

        var item = new LocalItem
        {
            Server = server,
            Title = title,
            ServerType = ApplicationHelper.GetEnum<ServerType>(PageType.ToString())
        };

        App.Current.NavigationManager.NavigateForJson(typeof(DetailPage), item);
    }

    private async void btnOpenDirectory_Click(object sender, RoutedEventArgs e)
    {
        var menuFlyout = (sender as MenuFlyoutItem);
        var localItem = (LocalItem) menuFlyout?.DataContext;
        var server = localItem.Server?.ToString();
        await Launcher.LaunchUriAsync(new Uri(server));
    }
}
