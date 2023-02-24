// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using WinUICommunity.LandingsPage.Controls;
using WinUICommunity.Shared.DataModel;

namespace TvTime.Views;
public sealed partial class HomeLandingsPage : Page
{
    public HomeLandingsPage()
    {
        this.InitializeComponent();
    }

    private void mainLandingsPage_Loaded(object sender, RoutedEventArgs e)
    {
        mainLandingsPage.GetDataAsync("DataModel/ControlInfoData.json", IncludedInBuildMode.CheckBasedOnIncludedInBuildProperty);
    }

    private void mainLandingsPage_OnItemClick(object sender, RoutedEventArgs e)
    {
        var args = (ItemClickEventArgs) e;
        var item = (ControlInfoDataItem) args.ClickedItem;

        NavigationViewHelper.GetCurrent().Navigate(typeof(ItemPage), item.UniqueId);
    }

    private void settingsTile_OnItemClick(object sender, RoutedEventArgs e)
    {
        NavigationViewHelper.GetCurrent().Navigate(typeof(SettingsPage));
    }

    private void aboutTile_OnItemClick(object sender, RoutedEventArgs e)
    {
        NavigationViewHelper.GetCurrent().Navigate(typeof(AboutPage));
    }
}
