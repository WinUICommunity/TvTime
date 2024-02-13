namespace TvTime.Views.ContentDialogs;
public sealed partial class ServerContentDialog : ContentDialog
{
    public string ServerTitle;

    public string ServerUrl;

    public object CmbServerTypeSelectedItem;

    public bool ServerActivation;

    public ServerViewModel ViewModel { get; set; }
    public ServerContentDialog()
    {
        this.InitializeComponent();
        XamlRoot = App.currentWindow.Content.XamlRoot;
        
        Loaded += ServerContentDialog_Loaded;
    }

    private void ServerContentDialog_Loaded(object sender, RoutedEventArgs e)
    {
        if (ViewModel != null)
        {
            RequestedTheme = ViewModel.themeService.GetElementTheme();
        }

        if (CmbServerTypeSelectedItem != null)
        {
            CmbServerType.SelectedItem = CmbServerType.Items.FirstOrDefault(x => ((ComboBoxItem)x).Tag.ToString() == CmbServerTypeSelectedItem.ToString());
        }
        else
        {
            CmbServerType.SelectedIndex = 0;
        }
    }
}
