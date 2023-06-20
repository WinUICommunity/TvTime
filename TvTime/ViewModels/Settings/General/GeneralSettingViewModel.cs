using Microsoft.UI.Xaml.Media.Animation;

namespace TvTime.ViewModels;
public partial class GeneralSettingViewModel : ObservableObject
{
    [ObservableProperty]
    public List<string> breadCrumbBarCollection = new();

    public GeneralSettingViewModel()
    {
        BreadCrumbBarCollection.Add("General");    
    }

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
        var item = sender as SettingsCard;
        if (item.Tag != null)
        {
            Type pageType = Application.Current.GetType().Assembly.GetType($"TvTime.Views.{item.Tag}");

            if (pageType != null)
            {
                DrillInNavigationTransitionInfo entranceNavigation = new DrillInNavigationTransitionInfo();
                App.Current.NavigationManager.NavigateForJson(pageType, item.Header, entranceNavigation);
            }
        }
    }
}
