namespace TvTime.ViewModels;
public partial class HomeLandingsViewModel : ObservableObject
{
    public IJsonNavigationViewService JsonNavigationViewService;
    public HomeLandingsViewModel(IJsonNavigationViewService jsonNavigationViewService)
    {
        JsonNavigationViewService = jsonNavigationViewService;
    }

    [RelayCommand]
    private void OnItemClick(RoutedEventArgs e)
    {
        var args = (ItemClickEventArgs) e;
        var item = (DataItem) args.ClickedItem;

        JsonNavigationViewService.NavigateTo(item.UniqueId+item.Parameter?.ToString(), item);
    }
}
