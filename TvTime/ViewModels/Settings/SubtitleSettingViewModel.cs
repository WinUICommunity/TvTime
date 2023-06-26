using Windows.Storage.Pickers;

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
        FolderPicker folderPicker = new();
        folderPicker.FileTypeFilter.Add("*");

        WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, WindowHelper.GetWindowHandleForCurrentWindow(App.Current.Window));

        StorageFolder folder = await folderPicker.PickSingleFolderAsync();
        if (folder is not null)
        {
            Settings.DefaultSubtitleDownloadPath = folder.Path;
            SubtitleDownloadPath = folder.Path;
        }
    }
}
