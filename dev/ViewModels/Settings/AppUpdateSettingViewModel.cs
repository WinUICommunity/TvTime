namespace TvTime.ViewModels;
public partial class AppUpdateSettingViewModel : ObservableObject
{
    [ObservableProperty]
    public string currentVersion;

    [ObservableProperty]
    public string lastUpdateCheck;

    [ObservableProperty]
    public bool isUpdateAvailable;

    [ObservableProperty]
    public bool isLoading;

    [ObservableProperty]
    public string loadingStatus = App.Current.ResourceHelper.GetString("AppUpdateSettingPage_InfoBarTitle/Title");

    private string changeLog = string.Empty;

    public AppUpdateSettingViewModel()
    {
        currentVersion = string.Format(App.Current.ResourceHelper.GetString("AppUpdateSettingViewModel_CurrentVersion"), App.Current.TvTimeVersion);
        lastUpdateCheck = Settings.LastUpdateCheck;
    }

    [RelayCommand]
    private async Task CheckForUpdateAsync()
    {
        IsLoading = true;
        IsUpdateAvailable = false;
        LoadingStatus = App.Current.ResourceHelper.GetString("AppUpdateSettingViewModel_StatusCheckUpdate");
        if (ApplicationHelper.IsNetworkAvailable())
        {
            LastUpdateCheck = DateTime.Now.ToShortDateString();
            Settings.LastUpdateCheck = DateTime.Now.ToShortDateString();

            try
            {
                var update = await UpdateHelper.CheckUpdateAsync("WinUICommunity", "TvTime", new Version(App.Current.TvTimeVersion));
                if (update.IsExistNewVersion)
                {
                    IsUpdateAvailable = true;
                    changeLog = update.Changelog;
                    LoadingStatus = string.Format(App.Current.ResourceHelper.GetString("AppUpdateSettingViewModel_StatusUpdateFound"), update.TagName, update.CreatedAt, update.PublishedAt);
                }
                else
                {
                    LoadingStatus = App.Current.ResourceHelper.GetString("AppUpdateSettingViewModel_StatusLatestVersion");
                }
            }
            catch (Exception ex)
            {
                LoadingStatus = ex.Message;
            }
        }
        else
        {
            LoadingStatus = App.Current.ResourceHelper.GetString("AppUpdateSettingViewModel_StatusErrorConnection");
        }
        IsLoading = false;
    }

    [RelayCommand]
    private async Task GetReleaseNotesAsync()
    {
        ContentDialog dialog = new ContentDialog()
        {
            Title = App.Current.ResourceHelper.GetString("AppUpdateSettingViewModel_ReleaseNoteContentDialogTitle"),
            CloseButtonText = App.Current.ResourceHelper.GetString("AppUpdateSettingViewModel_ReleaseNoteContentDialogCloseButton"),
            Content = new ScrollViewer
            {
                Content = new MarkdownTextBlock
                {
                    Text = changeLog,
                    CornerRadius = new CornerRadius(8),
                    Margin = new Thickness(10)
                },
                Margin = new Thickness(10)
            },
            Margin = new Thickness(10),
            DefaultButton = ContentDialogButton.Close,
            XamlRoot = App.currentWindow.Content.XamlRoot
        };

        await dialog.ShowAsyncQueue();
    }

    [RelayCommand]
    private async Task GoToUpdateAsync()
    {
        await Launcher.LaunchUriAsync(new Uri(Constants.TVTIME_REPO));
    }
}
