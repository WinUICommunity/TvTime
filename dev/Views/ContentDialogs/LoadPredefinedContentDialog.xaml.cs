namespace TvTime.Views.ContentDialogs;

public sealed partial class LoadPredefinedContentDialog : ContentDialog
{
    public IThemeService ThemeService { get; set; }
    public LoadPredefinedContentDialog()
    {
        this.InitializeComponent();
        XamlRoot = App.currentWindow.Content.XamlRoot;
        Loaded += LoadPredefinedContentDialog_Loaded;
    }

    private void LoadPredefinedContentDialog_Loaded(object sender, RoutedEventArgs e)
    {
        RequestedTheme = ThemeService.GetElementTheme();
    }
}
