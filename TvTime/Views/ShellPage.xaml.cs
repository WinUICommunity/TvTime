// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

namespace TvTime.Views;

public sealed partial class ShellPage : Page
{
    public ShellPage()
    {
        this.InitializeComponent();
        NavigationViewHelper.GetCurrent()
                                .WithAutoSuggestBox(controlsSearchBox)
                                .WithSettingsPage(typeof(SettingsPage))
                                .WithDefaultPage(typeof(HomeLandingsPage))
                                .Build("DataModel/ControlInfoData.json", rootFrame, NavigationViewControl);
    }

    private void controlsSearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        NavigationViewHelper.GetCurrent().AutoSuggestBoxQuerySubmitted(args);
    }

    private void OnNavigationViewSelectionChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
    {
        NavigationViewHelper.GetCurrent().OnNavigationViewSelectionChanged(args);
    }
}
