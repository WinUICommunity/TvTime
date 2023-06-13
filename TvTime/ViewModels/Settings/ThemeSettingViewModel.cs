using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace TvTime.ViewModels;
public partial class ThemeSettingViewModel : ObservableObject
{
    [RelayCommand]
    private void OnBackdropChanged(object sender)
    {
        App.Current.ThemeManager.OnBackdropComboBoxSelectionChanged(sender);
    }

    [RelayCommand]
    private void OnThemeChanged(object sender)
    {
        App.Current.ThemeManager.OnThemeComboBoxSelectionChanged(sender);
    }

    [RelayCommand]
    private async void OpenWindowsColorSettings()
    {
        _ = await Launcher.LaunchUriAsync(new Uri("ms-settings:colors"));
    }
}
