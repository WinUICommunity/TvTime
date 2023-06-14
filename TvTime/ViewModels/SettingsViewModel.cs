using Microsoft.UI.Xaml.Media.Animation;

namespace TvTime.ViewModels;
public partial class SettingsViewModel : ObservableRecipient
{
    [RelayCommand]
    private void GoToSettingPage(object sender)
    {
        var settingCard = sender as SettingsCard;
        if (settingCard.Tag != null)
        {
            Type pageType = Application.Current.GetType().Assembly.GetType($"TvTime.Views.{settingCard.Tag}");

            if (pageType != null)
            {
                DrillInNavigationTransitionInfo entranceNavigation = new DrillInNavigationTransitionInfo();
                App.Current.NavigationManager.NavigateForJson(pageType, settingCard.Header, entranceNavigation);
            }
        }
    }
}
