// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using System.Xml;
using WinUICommunity.LandingsPage.DataModel;

namespace TvTime.Views;
public sealed partial class HomeLandingsPage : Page
{
    public HomeLandingsPage()
    {
        this.InitializeComponent();
    }

    private void mainLandingsPage_Loaded(object sender, RoutedEventArgs e)
    {
        mainLandingsPage.GetControlInfoDataAsync("DataModel/ControlInfoData.json", IncludedInBuildMode.RealCheckBasedOnUniqeIdPath);
    }

    private void mainLandingsPage_OnItemClick(object sender, RoutedEventArgs e)
    {
        var args = (ItemClickEventArgs) e;
        var item = (ControlInfoDataItem) args.ClickedItem;

        Type pageType = Type.GetType(item.UniqueId);
        ShellPage.Instance.Navigate(pageType, null);
    }

    private void settingsTile_OnItemClick(object sender, RoutedEventArgs e)
    {
        ShellPage.Instance.Navigate(typeof(SettingsPage), null);
    }

    private void aboutTile_OnItemClick(object sender, RoutedEventArgs e)
    {
        ShellPage.Instance.Navigate(typeof(AboutPage), null);
    }
}
