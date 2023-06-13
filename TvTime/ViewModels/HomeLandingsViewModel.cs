using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace TvTime.ViewModels;
public partial class HomeLandingsViewModel : ObservableObject
{
    [RelayCommand]
    private void OnItemClick(RoutedEventArgs e)
    {
        var args = (ItemClickEventArgs) e;
        var item = (ControlInfoDataItem) args.ClickedItem;
        Type pageType = Type.GetType(item.UniqueId);

        if (pageType != null)
        {
            object parameter = null;
            App.Current.NavigationManager.NavigateForJson(pageType, parameter);
        }
    }
}
