namespace TvTime.ViewModels;
public partial class SubtitleSettingViewModel : ObservableObject
{
    [ObservableProperty]
    public string subtitleDownloadPath = Settings.DefaultSubtitleDownloadPath;

    [RelayCommand]
    private async void OnLaunchSubtitleLocation()
    {
        await Launcher.LaunchUriAsync(new Uri(Settings.DefaultSubtitleDownloadPath));
    }

    [RelayCommand]
    private async void OnChooseSubtitleLocation()
    {
        var folderPickerPath = await OpenFolderPicker();
        if (!string.IsNullOrEmpty(folderPickerPath))
        {
            Settings.DefaultSubtitleDownloadPath = folderPickerPath;
            SubtitleDownloadPath = folderPickerPath;
        }
    }
}
