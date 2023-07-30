using Microsoft.UI.Xaml.Media.Animation;

namespace TvTime.ViewModels;
public partial class SettingsViewModel : ObservableObject
{
    public IJsonNavigationViewService JsonNavigationViewService;
    public SettingsViewModel(IJsonNavigationViewService jsonNavigationViewService)
    {
        JsonNavigationViewService = jsonNavigationViewService;
    }

    [RelayCommand]
    private void GoToSettingPage(object sender)
    {
        var item = sender as SettingsCard;
        if (item.Tag != null)
        {
            Type pageType = Application.Current.GetType().Assembly.GetType($"TvTime.Views.{item.Tag}");

            if (pageType != null)
            {
                SlideNavigationTransitionInfo entranceNavigation = new SlideNavigationTransitionInfo();
                JsonNavigationViewService.NavigateTo(pageType, item.Header, false, entranceNavigation);
            }
        }
    }
}
