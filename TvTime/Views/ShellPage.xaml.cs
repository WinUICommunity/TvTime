// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.



using System.Reflection;

namespace TvTime.Views;

public sealed partial class ShellPage : Page
{
    public static ShellPage Instance { get; private set; }
    public ShellViewModel ViewModel { get; } = new ShellViewModel();

    public ShellPage()
    {
        this.InitializeComponent();
        Instance = this;
        ViewModel.InitializeNavigation(shellFrame, navigationView)
                    .WithKeyboardAccelerator(KeyboardAccelerators)
                    .WithDefaultPage(typeof(HomeLandingsPage))
                    .WithSettingsPage(typeof(SettingsPage));
        Loaded += ShellPage_Loaded;
    }

    private void ShellPage_Loaded(object sender, RoutedEventArgs e)
    {
        ViewModel.OnLoaded();
    }

    private void navigationView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
    {
        ViewModel.OnItemInvoked(args);
    }

    public void Navigate(Type page, object parameter)
    {
        shellFrame.Navigate(page, parameter);
    }
}
