using Microsoft.UI.Xaml.Navigation;
using WinUICommunity.LandingsPage.Controls;
using WinUICommunity.Shared.DataModel;

namespace TvTime.Views;
public sealed partial class TvTimeSectionPage : Page
{
    public TvTimeSectionPage()
    {
        this.InitializeComponent();
    }
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        sectionPage.GetDataAsync(e);
    }
    private void sectionPage_OnItemClick(object sender, RoutedEventArgs e)
    {
        var args = (ItemClickEventArgs) e;
        var item = (ControlInfoDataItem) args.ClickedItem;
        NavigationViewHelper.GetCurrent().Navigate(typeof(ItemPage), item.UniqueId);
    }
}
