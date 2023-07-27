using Microsoft.UI.Xaml.Media.Animation;

namespace TvTime.ViewModels;
public partial class SettingsViewModel : ObservableObject
{
    [RelayCommand]
    private void GoToSettingPage(object sender)
    {
        var item = sender as SettingsCard;
        if (item.Tag != null)
        {
            Type pageType = Application.Current.GetType().Assembly.GetType($"TvTime.Views.{item.Tag}");

            if (pageType != null)
            {
                DrillInNavigationTransitionInfo entranceNavigation = new DrillInNavigationTransitionInfo();
                App.Current.JsonNavigationViewService.NavigateTo(pageType, item.Header);
            }
        }
    }
}
