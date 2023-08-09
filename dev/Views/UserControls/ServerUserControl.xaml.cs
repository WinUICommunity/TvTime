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
        ViewModel = App.GetService<ServerViewModel>();
        this.InitializeComponent();
        DataContext = this;
    }

    private async void OnLoadPredefinedServer(object sender, RoutedEventArgs e)
    {
        ContentDialog contentDialog = new ContentDialog();
        contentDialog.XamlRoot = App.currentWindow.Content.XamlRoot;
        contentDialog.FlowDirection = ApplicationHelper.GetEnum<FlowDirection>(App.Current.ResourceHelper.GetString("MainPage_FlowDirection/FlowDirection"));
        contentDialog.Title = App.Current.ResourceHelper.GetString("ServerUC_LoadContentDialogTitle");
        contentDialog.Content = App.Current.ResourceHelper.GetString("ServerUC_LoadContentDialogContent");
        contentDialog.PrimaryButtonText = App.Current.ResourceHelper.GetString("ServerUC_LoadContentDialogPrimaryButton");
        contentDialog.CloseButtonText = App.Current.ResourceHelper.GetString("ServerUC_LoadContentDialogCloseButton");
        contentDialog.PrimaryButtonClick += async (s, e) =>
        {
            if (IsMediaServer)
            {
                ServerSettings.TVTimeServers?.Clear();
            }
            else
            {
                ServerSettings.SubtitleServers?.Clear();
            }

            ViewModel.DataListACV?.Clear();

            var filePath = "Assets/Files/TvTime-MediaServers.json";

            if (!IsMediaServer)
            {
                filePath = "Assets/Files/TvTime-SubtitleServers.json";
            }

            using var streamReader = File.OpenText(await FileLoaderHelper.GetPath(filePath));
            var json = await streamReader.ReadToEndAsync();
            var content = JsonConvert.DeserializeObject<ObservableCollection<ServerModel>>(json);
            if (content is not null)
            {
                if (IsMediaServer)
                {
                    ServerSettings.TVTimeServers = content;
                }
                else
                {
                    ServerSettings.SubtitleServers = content;
                }
                ViewModel.DataListACV = new(content);
                Status.Title = App.Current.ResourceHelper.GetString("ServerUC_LoadContentDialogStatus");
                Status.Severity = InfoBarSeverity.Success;
                Status.IsOpen = true;
            }
        };

        await contentDialog.ShowAsync();
    }

    private async void OnClearAllServer(object sender, RoutedEventArgs e)
    {
        ContentDialog contentDialog = new ContentDialog();
        contentDialog.XamlRoot = App.currentWindow.Content.XamlRoot;
        contentDialog.FlowDirection = ApplicationHelper.GetEnum<FlowDirection>(App.Current.ResourceHelper.GetString("MainPage_FlowDirection/FlowDirection"));
        contentDialog.Title = App.Current.ResourceHelper.GetString("ServerUC_ClearContentDialogTitle");
        contentDialog.Content = App.Current.ResourceHelper.GetString("ServerUC_ClearContentDialogContent");
        contentDialog.PrimaryButtonText = App.Current.ResourceHelper.GetString("ServerUC_ClearContentDialogPrimaryButton");
        contentDialog.CloseButtonText = App.Current.ResourceHelper.GetString("ServerUC_ClearContentDialogCloseButton");
        contentDialog.PrimaryButtonClick += (s, e) =>
        {
            if (IsMediaServer)
            {
                ServerSettings.TVTimeServers?.Clear();
            }
            else
            {
                ServerSettings.SubtitleServers?.Clear();
            }

            ViewModel.DataListACV?.Clear();
            Status.Title = App.Current.ResourceHelper.GetString("ServerUC_ClearContentDialogStatus");
            Status.Severity = InfoBarSeverity.Success;
            Status.IsOpen = true;
        };

        await contentDialog.ShowAsync();
    }
}
