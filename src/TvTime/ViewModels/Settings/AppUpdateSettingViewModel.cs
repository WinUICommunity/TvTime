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
    public string loadingStatus = App.Current.Localizer.GetLocalizedString("AppUpdateSettingPage_InfoBarTitle");

    private string changeLog = string.Empty;

    public AppUpdateSettingViewModel()
    {
        currentVersion = $"Current Version v{App.Current.TvTimeVersion}";
        lastUpdateCheck = Settings.LastUpdateCheck;
    }

    [RelayCommand]
    private async void CheckForUpdateAsync()
    {
        IsLoading = true;
        IsUpdateAvailable = false;
        LoadingStatus = "Checking for new updates";
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
                    LoadingStatus = $"We found a new version {update.TagName} that created in {update.CreatedAt} and published in {update.PublishedAt} ";
                }
                else
                {
                    LoadingStatus = "You are using Latest version.";
                }
            }
            catch (Exception ex)
            {
                LoadingStatus = ex.Message;
            }
        }
        else
        {
            LoadingStatus = "Error in Connection";
        }
        IsLoading = false;
    }

    [RelayCommand]
    private async void GetReleaseNotesAsync()
    {
        ContentDialog dialog = new ContentDialog()
        {
            Title = "Release Notes",
            CloseButtonText = "Close",
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
    private async void GoToUpdateAsync()
    {
        await Launcher.LaunchUriAsync(new Uri(Constants.TVTIME_REPO));
    }
}
