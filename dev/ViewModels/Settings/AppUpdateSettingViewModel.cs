using TvTime.Views.ContentDialogs;

using Windows.System;

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
    public bool isCheckButtonEnabled = true;

    [ObservableProperty]
    public string loadingStatus = "Status";

    private string ChangeLog = string.Empty;

    private IThemeService themeService;
    public AppUpdateSettingViewModel(IThemeService themeService)
    {
        CurrentVersion = $"Current Version v{App.Current.AppVersion}";
        LastUpdateCheck = Settings.LastUpdateCheck;
        this.themeService = themeService;
    }

    [RelayCommand]
    private async Task CheckForUpdateAsync()
    {
        IsLoading = true;
        IsUpdateAvailable = false;
        IsCheckButtonEnabled = false;
        LoadingStatus = "Checking for new version";
        if (NetworkHelper.IsNetworkAvailable())
        {
            LastUpdateCheck = DateTime.Now.ToShortDateString();
            Settings.LastUpdateCheck = DateTime.Now.ToShortDateString();

            try
            {
                string username = "WinUICommunity";
                string repo = "TvTime";
                var update = await UpdateHelper.CheckUpdateAsync(username, repo, new Version(App.Current.AppVersion));
                if (update.IsExistNewVersion)
                {
                    IsUpdateAvailable = true;
                    ChangeLog = update.Changelog;
                    LoadingStatus = $"We found a new version {update.TagName} Created at {update.CreatedAt} and Published at {update.PublishedAt}";
                }
                else
                {
                    LoadingStatus = "You are using latest version";
                }
            }
            catch (Exception ex)
            {
                LoadingStatus = ex.Message;
                IsLoading = false;
                IsCheckButtonEnabled = true;
                Logger?.Error(ex, "AppUpdateSettingViewModel: CheckForUpdateAsync");
            }
        }
        else
        {
            LoadingStatus = "Error Connection";
        }
        IsLoading = false;
        IsCheckButtonEnabled = true;
    }

    [RelayCommand]
    private async Task GoToUpdateAsync()
    {
        await Launcher.LaunchUriAsync(new Uri("https://github.com/WinUICommunity/TvTime/releases"));
    }

    [RelayCommand]
    private async Task GetReleaseNotesAsync()
    {
        var dialog = new ChangeLogContentDialog();
        dialog.ChangeLog = ChangeLog;
        dialog.ThemeService = themeService;

        await dialog.ShowAsyncQueue();
    }
}
