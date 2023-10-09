namespace TvTime.ViewModels;
public partial class GeneralSettingViewModel : ObservableObject
{
    [RelayCommand]
    private void OnNavigationViewPaneDisplayModeChanged()
    {
        MainPage.Instance.RefreshNavigationViewPaneDisplayMode();
    }
}
