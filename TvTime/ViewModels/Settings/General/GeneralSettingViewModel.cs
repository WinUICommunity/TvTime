using Microsoft.UI.Xaml.Media.Animation;

namespace TvTime.ViewModels;
public partial class GeneralSettingViewModel : ObservableObject
{
    [RelayCommand]
    private void OnIconPackChanged(object sender)
    {
        var cmbIconPack = sender as ComboBox;
        if (cmbIconPack != null)
        {
            var selectedItem = cmbIconPack.SelectedItem as ComboBoxItem;
            var iconPack = ApplicationHelper.GetEnum<IconPackType>(selectedItem?.Tag?.ToString());
            Settings.IconPack = iconPack;
        }
    }

    [RelayCommand]
    private void GoToStyleSettingPage(object sender)
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
