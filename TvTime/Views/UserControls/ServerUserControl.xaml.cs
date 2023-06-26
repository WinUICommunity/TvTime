using TvTime.ViewModels;

namespace TvTime.Views;
public sealed partial class ServerUserControl : UserControl
{
    public bool IsMediaServer
    {
        get => (bool) GetValue(IsMediaServerProperty);
        set => SetValue(IsMediaServerProperty, value);
    }

    public static readonly DependencyProperty IsMediaServerProperty =
        DependencyProperty.Register("IsMediaServer", typeof(bool), typeof(ServerUserControl), new PropertyMetadata(true));


    public ServerViewModel ViewModel { get; }

    public ServerUserControl()
    {
        ViewModel = App.Current.Services.GetService<ServerViewModel>();
        this.InitializeComponent();
        DataContext = this;
    }

    private async void OnLoadPredefinedServer(object sender, RoutedEventArgs e)
    {
        ContentDialog contentDialog = new ContentDialog();
        contentDialog.XamlRoot = App.Current.Window.Content.XamlRoot;
        contentDialog.Title = "Load Predefined Servers";
        contentDialog.Content = "Are you sure to load predefined servers? This will overwrite all your servers!";
        contentDialog.PrimaryButtonText = "Yes";
        contentDialog.CloseButtonText = "No";
        contentDialog.PrimaryButtonClick += async (s, e) =>
        {
            if (IsMediaServer)
            {
                Settings.TVTimeServers?.Clear();
            }
            else
            {
                Settings.SubtitleServers?.Clear();
            }

            ViewModel.DataListACV?.Clear();

            var filePath = "Assets/Files/TvTime-MediaServers.json";

            if (!IsMediaServer)
            {
                filePath = "Assets/Files/TvTime-SubtitleServers.json";
            }

            using var streamReader = File.OpenText(await GetFilePath(filePath));
            var json = await streamReader.ReadToEndAsync();
            var content = JsonConvert.DeserializeObject<ObservableCollection<ServerModel>>(json);
            if (content is not null)
            {
                if (IsMediaServer)
                {
                    Settings.TVTimeServers = content;
                }
                else
                {
                    Settings.SubtitleServers = content;
                }
                ViewModel.DataListACV = new(content);
                Status.Title = "Predefined Servers Loaded Successfully";
                Status.Severity = InfoBarSeverity.Success;
                Status.IsOpen = true;
            }
        };

        await contentDialog.ShowAsync();
    }

    private async void OnClearAllServer(object sender, RoutedEventArgs e)
    {
        ContentDialog contentDialog = new ContentDialog();
        contentDialog.XamlRoot = App.Current.Window.Content.XamlRoot;
        contentDialog.Title = "Clear All Servers";
        contentDialog.Content = "Are you sure to clear all servers?";
        contentDialog.PrimaryButtonText = "Yes";
        contentDialog.CloseButtonText = "No";
        contentDialog.PrimaryButtonClick += (s, e) =>
        {
            if (IsMediaServer)
            {
                Settings.TVTimeServers?.Clear();
            }
            else
            {
                Settings.SubtitleServers?.Clear();
            }

            ViewModel.DataListACV?.Clear();
            Status.Title = "All Servers Removed Successfully";
            Status.Severity = InfoBarSeverity.Success;
            Status.IsOpen = true;
        };

        await contentDialog.ShowAsync();
    }
}
