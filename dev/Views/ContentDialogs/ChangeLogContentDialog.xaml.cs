namespace TvTime.Views.ContentDialogs;

public sealed partial class ChangeLogContentDialog : ContentDialog
{
    public string ChangeLog;
    public IThemeService ThemeService;
    public ChangeLogContentDialog()
    {
        this.InitializeComponent();
        XamlRoot = App.currentWindow.Content.XamlRoot;
        Loaded += ChangeLogContentDialog_Loaded;
    }

    private void ChangeLogContentDialog_Loaded(object sender, RoutedEventArgs e)
    {
        RequestedTheme = ThemeService.GetCurrentTheme();
    }
}
