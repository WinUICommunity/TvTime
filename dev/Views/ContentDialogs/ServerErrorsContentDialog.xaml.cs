namespace TvTime.Views.ContentDialogs;

public sealed partial class ServerErrorsContentDialog : ContentDialog
{
    public IThemeService ThemeService { get; set; }
    public ObservableCollection<ExceptionModel> Exceptions { get; set; }
    public ServerErrorsContentDialog()
    {
        this.InitializeComponent();
        XamlRoot = App.currentWindow.Content.XamlRoot;
        Loaded += ServerErrorsContentDialog_Loaded;
    }

    private void ServerErrorsContentDialog_Loaded(object sender, RoutedEventArgs e)
    {
        RequestedTheme = ThemeService.GetCurrentTheme();
    }
}
