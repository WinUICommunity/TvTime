namespace TvTime.ViewModels;
public partial class ThemeSettingViewModel : ObservableObject
{
    [RelayCommand]
    private void OnBackdropChanged(object sender)
    {
        App.Current.ThemeService.OnBackdropComboBoxSelectionChanged(sender);
    }

    [RelayCommand]
    private void OnThemeChanged(object sender)
    {
        App.Current.ThemeService.OnThemeComboBoxSelectionChanged(sender);
    }

    [RelayCommand]
    private async void OpenWindowsColorSettings()
    {
        _ = await Launcher.LaunchUriAsync(new Uri("ms-settings:colors"));
    }
}
