namespace TvTime.Views.ContentDialogs;

public sealed partial class GoToServerContentDialog : ContentDialog
{
    public IJsonNavigationViewService JsonNavigationViewService { get; set; }
    public IThemeService ThemeService { get; set; }
    public GoToServerContentDialog()
    {
        this.InitializeComponent();
        XamlRoot = App.currentWindow.Content.XamlRoot;
        Loaded += GoToServerContentDialog_Loaded;
    }

    private void GoToServerContentDialog_Loaded(object sender, RoutedEventArgs e)
    {
        RequestedTheme = ThemeService.GetElementTheme();
    }

    private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        JsonNavigationViewService?.NavigateTo(typeof(ServerPage));
    }
}
