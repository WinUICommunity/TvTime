// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using WinUICommunity.LandingsPage.Controls;

namespace TvTime.Views;

public sealed partial class ShellPage : Page
{
    public ShellPage()
    {
        this.InitializeComponent();
        Loaded += ShellPage_Loaded;
    }

    private void ShellPage_Loaded(object sender, RoutedEventArgs e)
    {
        NavigationViewHelper.GetCurrent()
                        .WithAutoSuggestBox(controlsSearchBox)
                        .WithSettingsPage(typeof(SettingsPage))
                        .WithDefaultPage(typeof(HomeLandingsPage))
                        .Build("DataModel/ControlInfoData.json", rootFrame, NavigationViewControl);
    }
    private void controlsSearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        NavigationViewHelper.GetCurrent().AutoSuggestBoxQuerySubmitted(args, typeof(ItemPage));
    }

    private void OnNavigationViewSelectionChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
    {
        NavigationViewHelper.GetCurrent().OnNavigationViewSelectionChanged(args, typeof(TvTimeSectionPage), typeof(ItemPage));
    }
}
