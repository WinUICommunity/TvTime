namespace TvTime.ViewModels;
public partial class HomeLandingsViewModel : ObservableObject
{
    [RelayCommand]
    private void OnItemClick(RoutedEventArgs e)
    {
        var args = (ItemClickEventArgs) e;
        var item = (DataItem) args.ClickedItem;

        App.Current.JsonNavigationViewService.NavigateTo(item.UniqueId, item);
    }
}
