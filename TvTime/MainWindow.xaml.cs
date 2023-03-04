// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using TvTime.Views;

namespace TvTime;

public sealed partial class MainWindow : Window
{
    public static MainWindow Instance { get; private set; }
    public string TvTimeVersion { get; set; }

    public MainWindow()
    {
        this.InitializeComponent();
        Instance = this;
        TitleBarHelper.Initialize(this, TitleTextBlock, AppTitleBar, LeftPaddingColumn, IconColumn, TitleColumn, LeftDragColumn, SearchColumn, RightDragColumn, RightPaddingColumn);
        TvTimeVersion = $"TvTime v{VersionHelper.GetVersion()}";
    }
    private void controlsSearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        NavigationViewHelper.GetCurrent().AutoSuggestBoxQuerySubmitted(args);
    }

    private void OnNavigationViewSelectionChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
    {
        NavigationViewHelper.GetCurrent().OnNavigationViewSelectionChanged(args);
    }

    private void Grid_Loaded(object sender, RoutedEventArgs e)
    {
        NavigationViewHelper.GetCurrent()
                                .WithAutoSuggestBox(controlsSearchBox)
                                .WithSettingsPage(typeof(SettingsPage))
                                .WithDefaultPage(typeof(HomeLandingsPage))
                                .Build("DataModel/ControlInfoData.json", rootFrame, NavigationViewControl);
    }
}
