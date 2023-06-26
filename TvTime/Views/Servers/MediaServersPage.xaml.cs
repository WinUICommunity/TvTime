using TvTime.ViewModels;

namespace TvTime.Views;
public sealed partial class MediaServersPage : Page
{
    public MediaServerViewModel ViewModel { get; }

    public MediaServersPage()
    {
        ViewModel = App.Current.Services.GetService<MediaServerViewModel>();
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
            Settings.Servers?.Clear();
            ViewModel.DataListACV?.Clear();

            var filePath = "Assets/Files/TvTime-Servers.json";
            using var streamReader = File.OpenText(await GetFilePath(filePath));
            var json = await streamReader.ReadToEndAsync();
            var content = JsonConvert.DeserializeObject<ObservableCollection<ServerModel>>(json);
            if (content is not null)
            {
                Settings.Servers = content;
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
            Settings.Servers?.Clear();
            ViewModel.DataListACV?.Clear();
            Status.Title = "All Servers Removed Successfully";
            Status.Severity = InfoBarSeverity.Success;
            Status.IsOpen = true;
        };

        await contentDialog.ShowAsync();
    }
}
