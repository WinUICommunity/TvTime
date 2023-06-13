using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace TvTime.ViewModels;
public partial class AboutUsSettingViewModel : ObservableObject
{
    [ObservableProperty]
    public string tvTimeVersion = $"TvTime v{App.Current.TvTimeVersion}";

    [RelayCommand]
    private async void OnGoToGithub()
    {
        await Launcher.LaunchUriAsync(new Uri(Constants.TVTIME_REPO));
    }
}
